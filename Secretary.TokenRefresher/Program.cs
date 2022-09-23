using Secretary.Cache;
using Secretary.Configuration;
using Secretary.Documents;
using Secretary.HealthCheck;
using Secretary.Storage;
using Secretary.Telegram;
using Secretary.Yandex.Authentication;

namespace Secretary.TokenRefresher;

internal class Program
{
    private static Database _database = null!;
    private static ICacheService _cacheService = null!;
    private static IYandexAuthenticator _yandexAuthenticator = null!;
    private static HealthCheckService _healthCheckService = null!;
    private static TokenRefresher _refresher = null!;

    private static ITelegramClient _telegramClient = null!;
    private static readonly CancellationTokenSource CancellationTokenSource = new ();

    private static async Task Main(string[] args)
    {
        DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);

        _database = new Database();

        _cacheService = new RedisCacheService(Config.Instance.RedisHost);
        _healthCheckService = new HealthCheckService(_cacheService);
        _telegramClient = new TelegramClient(Config.Instance.TelegramApiKey, CancellationTokenSource.Token);
        _yandexAuthenticator = new YandexAuthenticator(Config.Instance.MailConfig);
        _refresher = new TokenRefresher(
            _yandexAuthenticator,
            _database.UserStorage,
            _telegramClient
        );

        _ = _refresher.RunThread();
        
        await RunHealthChecker();
    }

    private static async Task RunHealthChecker()
    {
        while (!CancellationTokenSource.IsCancellationRequested)
        {
            var reminderHealthData = _refresher.GetHealthData();

            await _healthCheckService.SaveData(reminderHealthData);

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}