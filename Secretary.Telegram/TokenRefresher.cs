using Secretary.Configuration;
using Secretary.Logging;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Yandex.Authentication;
using Serilog;

namespace Secretary.Telegram;

public class TokenRefresher
{
    private readonly ILogger _logger = LogPoint.GetLogger<TokenRefresher>();
    
    private readonly IYandexAuthenticator _yandexAuthenticator;
    private readonly Database _database;
    private readonly CancellationToken _cancellationToken;
    private Task? _refresher;

    private IUserStorage UserStorage => _database.UserStorage;
    
    public TokenRefresher(Config config, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _database = new Database();

        _yandexAuthenticator = new YandexAuthenticator(config.MailConfig);
    }

    public void RunThread()
    {
        _logger.Information("Run refresh token thread");

        _refresher = this.RefreshRoutine();
    }

    private async Task RefreshRoutine()
    {
        var now = DateTime.UtcNow;

        if (now.Day == 19 && now.Hour == 23)
        {
            await RefreshTokens();
        }
        else
        {
            await Task.Delay(60 * 60 * 1000);
        }
    }

    private async Task RefreshTokens()
    {
        var count = await UserStorage.GetCount();

        const int step = 10;

        for (int i = 0; i < count; i += step)
        {
            var users = await UserStorage.GetUsers(i, step);

            await RefreshTokens(users);
        }
    }

    private async Task RefreshTokens(IEnumerable<User> users)
    {
        foreach (var user in users)
        {
            try
            {
                var refreshed = await RefreshToken(user);

                if (refreshed)
                {
                     await Task.Delay(30 * 1000);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Could not refresh token for user {user.ChatId}");
            }
        }
    }

    private async Task<bool> RefreshToken(User user)
    {
        if (user.RefreshToken == null)
        {
            _logger.Information($"Skip refresh for user {user.ChatId}");
            
            return false;
        }

        _logger.Debug($"Run refresh for user {user.ChatId}");

        var tokenData = await _yandexAuthenticator.RefreshToken(user.RefreshToken, _cancellationToken);

        user.RefreshToken = tokenData!.refresh_token;
        user.AccessToken = tokenData!.access_token;
        user.TokenCreationTime = DateTime.UtcNow;
        user.TokenExpirationSeconds = tokenData!.expires_in;

        await UserStorage.SetUser(user);
        
        _logger.Debug($"Refreshed for user {user.ChatId}");

        return true;
    }
}