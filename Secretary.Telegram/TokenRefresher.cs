using Secretary.Logging;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Exceptions;
using Serilog;

namespace Secretary.Telegram;

public class TokenRefresher
{
    public delegate Task AsyncUserDelegate(User user);
    
    public event AsyncUserDelegate OnUserInvalidToken;

    private readonly ILogger _logger = LogPoint.GetLogger<TokenRefresher>();
    
    private readonly IYandexAuthenticator _yandexAuthenticator;
    private readonly IUserStorage _userStorage;
    private readonly CancellationToken _cancellationToken;

    public bool ShouldSkipDelay { get; set; } = false;
    
    public DateOnly LastRefreshDate { get; set; }
    public DateOnly NextRefreshDate => GetNextUpdateDate(DateTime.UtcNow);
    public DateTime LastDateCheck { get; set; }
    
    public TokenRefresher(IYandexAuthenticator yandexAuthenticator, IUserStorage userStorage, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _userStorage = userStorage;

        _yandexAuthenticator = yandexAuthenticator;
    }

    public async Task RunThread()
    {
        _logger.Information("Run refresh token thread");

        while (!_cancellationToken.IsCancellationRequested)
        {
            await this.Refresh();
        }
    }

    public async Task Refresh()
    {
        try
        {
            var now = DateTime.UtcNow;

            LastDateCheck = now;
            
            if (ItsTimeToRefresh(NextRefreshDate, LastRefreshDate, now))
            {
                _logger.Information("Run token refreshing");
                
                await RefreshTokensForAllUsers();

                LastRefreshDate = NextRefreshDate;
            }
            
            await Sleep(TimeSpan.FromMinutes(5), _cancellationToken);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Could not refresh tokens");
            await Sleep(TimeSpan.FromMinutes(5), _cancellationToken);
        }
    }

    public async Task RefreshTokensForAllUsers()
    {
        var count = await _userStorage.GetCount();

        const int step = 10;

        for (int i = 0; i < count; i += step)
        {
            var users = await _userStorage.GetUsers(i, step);

            await RefreshTokens(users);
        }
    }

    public async Task RefreshTokens(IEnumerable<User> users)
    {
        foreach (var user in users)
        {
            if (user.RefreshToken == null)
            {
                _logger.Debug($"Skip refresh for user {user.ChatId}");

                continue;
            }
            
            await RefreshToken(user);

            await Sleep(TimeSpan.FromSeconds(30), _cancellationToken);
        }
    }

    public async Task RefreshToken(User user)
    {
        try
        {
            _logger.Debug($"Run refresh for user {user.ChatId}");

            var tokenData = await _yandexAuthenticator.RefreshToken(user.RefreshToken!, _cancellationToken);

            user.RefreshToken = tokenData!.refresh_token;
            user.AccessToken = tokenData.access_token;
            user.TokenCreationTime = DateTime.UtcNow;
            user.TokenExpirationSeconds = tokenData!.expires_in;

            await _userStorage.UpdateUser(user);

            _logger.Debug($"Refreshed for user {user.ChatId}");
        }
        catch (YandexAuthenticationException e)
        {
            if (e.Message == "Refresh token expired")
            {
                await OnUserInvalidToken.Invoke(user);
                _logger.Warning($"{user.ChatId} has expired token");
                
                return;
            }
            
            _logger.Warning(e, $"Could not update token for {user.ChatId}");
        }
        catch (Exception e)
        {
            _logger.Warning(e, $"Could not update token for {user.ChatId}");
        }
    }

    public Task Sleep(TimeSpan span, CancellationToken cancellationToken)
    {
        if (ShouldSkipDelay)
        {
            return Task.CompletedTask;;
        }
        
        return Task.Delay(span, cancellationToken);
    }

    public bool ItsTimeToRefresh(DateOnly nextDate, DateOnly prevDate, DateTime now)
    {
        if (nextDate == prevDate) return false;
        
        if (nextDate == DateOnly.FromDateTime(now) && now.Hour == 0)
        {
            return true;
        }

        return false;
    }
    
    public DateOnly GetNextUpdateDate(DateTime now)
    {
        var monthDiff = now.Month % 3;

        var nextDate = now.AddMonths(monthDiff);
        
        var result = new DateOnly(nextDate.Year, nextDate.Month, 1);

        if (result < DateOnly.FromDateTime(now))
        {
            result = result.AddMonths(3);
        }

        return result;
    }
}