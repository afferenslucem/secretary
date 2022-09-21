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
    private readonly ITelegramClient _telegramClient;
    private readonly IUserStorage _userStorage;
    private readonly CancellationToken _cancellationToken;
    
    public TokenRefresher(IYandexAuthenticator yandexAuthenticator, ITelegramClient telegramClient, IUserStorage userStorage, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _userStorage = userStorage;

        _yandexAuthenticator = yandexAuthenticator;
        _telegramClient = telegramClient;
    }

    public async Task RunThread()
    {
        _logger.Information("Run refresh token thread");

        while (!_cancellationToken.IsCancellationRequested)
        {
            await this.RefreshRoutine();
        }
    }

    private async Task RefreshRoutine()
    {
        try
        {
            var now = DateTime.UtcNow;

            _logger.Information($"Now UTC: {now}");

            if ((now.Month % 3 == 0) && (now.Day == 21) && (now.Hour == 4))
            {
                _logger.Information("Run token refreshing");
                await RefreshTokens();
                
                await Task.Delay(TimeSpan.FromDays(1), _cancellationToken);
                _logger.Information("Tokens refreshed");
            }
            else
            {
                await Task.Delay(TimeSpan.FromMinutes(15), _cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, "Could not refresh tokens");
        }
    }

    private async Task RefreshTokens()
    {
        var count = await _userStorage.GetCount();

        const int step = 10;

        for (int i = 0; i < count; i += step)
        {
            var users = await _userStorage.GetUsers(i, step);

            await RefreshTokens(users);
        }
    }

    private async Task RefreshTokens(IEnumerable<User> users)
    {
        foreach (var user in users)
        {
            try
            {
                if (user.RefreshToken == null)
                {
                    _logger.Debug($"Skip refresh for user {user.ChatId}");

                    continue;
                }
                
                await RefreshToken(user);
                await Task.Delay(TimeSpan.FromSeconds(30), _cancellationToken);
            }
            catch (Exception e)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), _cancellationToken);
                
                _logger.Error(e, $"Could not refresh token for user {user.ChatId}");
            }
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
            }
            
            throw;
        }
    }
}