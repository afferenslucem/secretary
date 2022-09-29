using Secretary.HealthCheck.Data;
using Secretary.Logging;
using Secretary.Scheduler;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Exceptions;
using Serilog;

namespace Secretary.TokenRefresher;

public class TokenRefresher
{
    public static string Version = "v1.1.0";
    public static DateTime Uptime = DateTime.UtcNow;
    
    private readonly ILogger _logger = LogPoint.GetLogger<TokenRefresher>();
    
    private readonly IYandexAuthenticator _yandexAuthenticator;
    private readonly IUserStorage _userStorage;
    private readonly ITelegramClient _telegramClient;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public bool ShouldSkipDelay { get; set; }

    private TimeWaiter _timeWaiter;
    public DateTime NextRefreshDate => _timeWaiter.TargetDate;
    public DateTime LastActivityDateCheck { get; set; }
    
    public TokenRefresher(
        IYandexAuthenticator yandexAuthenticator, 
        IUserStorage userStorage,
        ITelegramClient telegramClient
    ) {
        _logger.Information($"Version: {Version}");
        _logger.Information($"Uptime : {Uptime}");
        
        _cancellationTokenSource = new CancellationTokenSource();
        _userStorage = userStorage;
        _telegramClient = telegramClient;
        _yandexAuthenticator = yandexAuthenticator;
        
        _timeWaiter = new TimeWaiter()
        {
            TargetDate = GetNextUpdateDate(DateTime.UtcNow)
        };

        _timeWaiter.OnTime += Routine;
    }

    public async Task RunThread()
    {
        _logger.Information("Run refresh token thread");

        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            await CheckTimer();
        }
    }

    public async Task CheckTimer()
    {
        try
        {
            _logger.Debug("Check time");
            
            LastActivityDateCheck = DateTime.UtcNow;
            
            _timeWaiter.Check();
            
            await Sleep(TimeSpan.FromMinutes(1), _cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Could not refresh tokens");
            await Sleep(TimeSpan.FromMinutes(5), _cancellationTokenSource.Token);
        }
    }

    public async Task Routine()
    {
        _logger.Information("Run token refreshing");
                
        await RefreshTokensForAllUsers();

        _timeWaiter.TargetDate = GetNextUpdateDate(DateTime.UtcNow.AddDays(1));
    }

    public async Task RefreshTokensForAllUsers()
    {
        var users = await _userStorage.GetUsers(user => user.RefreshToken != null);

        foreach (var user in users)
        {
            await RefreshToken(user);

            await Sleep(TimeSpan.FromSeconds(10), _cancellationTokenSource.Token);
        }
    }

    public async Task RefreshToken(User user)
    {
        try
        {
            _logger.Debug($"Run refresh for user {user.ChatId}");

            var tokenData = await _yandexAuthenticator.RefreshToken(user.RefreshToken!, _cancellationTokenSource.Token);

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
                await _userStorage.RemoveTokens(user.ChatId);
                await _telegramClient.SendMessage(
                    user.ChatId, 
                    "У вас истек токен для отправки почты!\n\n" +
                    $"Выполните команду /registermail для адреса {user.Email}"
                );
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
    
    public DateTime GetNextUpdateDate(DateTime now)
    {
        var monthDiff = now.Month % 3;

        var nextDate = now.AddMonths(monthDiff);
        
        var result = new DateTime(nextDate.Year, nextDate.Month, 1);

        if (result < now)
        {
            result = result.AddMonths(3);
        }

        _logger.Information($"Next refresh date: {result}");
        
        return result.ToUniversalTime();
    }

    public RefresherHealthData GetHealthData()
    {
        var result = new RefresherHealthData();

        result.PingTime = LastActivityDateCheck;
        result.NextRefreshDate = NextRefreshDate;
        result.DeployTime = Uptime;
        result.Version = Version;

        return result;
    }
}