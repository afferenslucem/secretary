using System;
using System.Threading;
using System.Threading.Tasks;
using Secretary.Cache;
using Secretary.Configuration;
using Secretary.Documents;
using Secretary.HealthCheck;
using Secretary.Storage;
using Secretary.Telegram;
using Secretary.Telegram.Sessions;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Mail;

namespace Secretary.TelegramBot;

internal class Program
{
    private static Database _database = null!;
    private static Telegram.TelegramBot _telegramBot = null!;
    private static ICacheService _cacheService = null!;
    private static ISessionStorage _sessionStorage = null!;
    private static HealthCheckService _healthCheckService = null!;
    private static IYandexAuthenticator _yandexAuthenticator = null!;
    private static IMailSender _mailSender = null!;

    private static ITelegramClient _telegramClient = null!;
    private static readonly CancellationTokenSource CancellationTokenSource = new ();

    private static async Task Main(string[] args)
    {
        DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);

        _database = new Database();
        _database.MigrateDatabase();

        _cacheService = new RedisCacheService(Config.Instance.RedisHost);
        _healthCheckService = new HealthCheckService(_cacheService);
        _sessionStorage = new SessionStorage(_cacheService);
        _yandexAuthenticator = new YandexAuthenticator(Config.Instance.MailConfig);
        _mailSender = new MailSender();
        _telegramClient = new TelegramClient(Config.Instance.TelegramApiKey, CancellationTokenSource.Token);

        _telegramBot = new Telegram.TelegramBot(
            _database, 
            _cacheService, 
            _sessionStorage, 
            _yandexAuthenticator,
            _mailSender,
            _telegramClient
        );
        
        _telegramBot.Init();
        
        _ = _telegramBot.Listen();
        
        await RunHealthChecker();
    }

    private static async Task RunHealthChecker()
    {
        while (!CancellationTokenSource.IsCancellationRequested)
        {
            var botHealthData = _telegramBot.GetHealthData();

            await _healthCheckService.SaveData(botHealthData);

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}