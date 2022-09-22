using System;
using System.Threading;
using System.Threading.Tasks;
using Secreatry.HealthCheck;
using Secreatry.HealthCheck.Data;
using Secretary.Cache;
using Secretary.Configuration;
using Secretary.Documents;
using Secretary.Storage;
using Secretary.Telegram;
using Secretary.Telegram.Sessions;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Mail;

namespace Secretary.Console;

internal class Program
{
    public static Database Database = null!;
    public static TelegramBot TelegramBot = null!;
    public static ICacheService CacheService = null!;
    public static ISessionStorage SessionStorage = null!;
    public static HealthCheckService HealthCheckService = null!;
    public static IYandexAuthenticator YandexAuthenticator = null!;
    public static IMailClient MailClient = null!;
    public static ITelegramClient TelegramClient = null!;
    public static TokenRefresher Refresher = null!;

    public static CancellationTokenSource CancellationTokenSource = new ();

    private static async Task Main(string[] args)
    {
        DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);

        Database = new Database();
        Database.MigrateDatabase();

        CacheService = new RedisCacheService(Config.Instance.RedisHost);
        HealthCheckService = new HealthCheckService(CacheService);
        SessionStorage = new SessionStorage(CacheService);
        YandexAuthenticator = new YandexAuthenticator(Config.Instance.MailConfig);
        MailClient = new MailClient();
        TelegramClient = new TelegramClient(Config.Instance.TelegramApiKey, CancellationTokenSource.Token);

        TelegramBot = new TelegramBot(
            Database, 
            CacheService, 
            SessionStorage, 
            YandexAuthenticator,
            MailClient,
            TelegramClient
        );
        
        Refresher = new TokenRefresher(
            YandexAuthenticator, 
            Database.UserStorage, 
            TelegramClient
        );
        
        
        TelegramBot.Init();
        
        _ = Refresher.RunThread();
        _ = TelegramBot.Listen();
        
        await RunHealthChecker();
    }

    private static async Task RunHealthChecker()
    {
        while (!CancellationTokenSource.IsCancellationRequested)
        {
            var data = new HealthData();

            data.BotHealthData = TelegramBot.GetHealthData();
            data.RefresherHealthData = Refresher.GetHealthData();

            await HealthCheckService.SaveData(data);

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}