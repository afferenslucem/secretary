using System;
using System.Threading.Tasks;
using Secreatry.HealthCheck;
using Secretary.Cache;
using Secretary.Configuration;
using Secretary.Documents;
using Secretary.Storage;
using Secretary.Telegram;
using Secretary.Telegram.Sessions;

namespace Secretary.Console;

internal class Program
{
    public static Database Database = null!;
    public static TelegramBot TelegramBot = null!;
    public static ICacheService CacheService = null!;
    public static ISessionStorage SessionStorage = null!;
    public static HealthCheckService HealthCheckService = null!;

    private static async Task Main(string[] args)
    {
        DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);

        Database = new Database();
        Database.MigrateDatabase();

        CacheService = new RedisCacheService(Config.Instance.RedisHost);

        HealthCheckService = new HealthCheckService(CacheService);

        SessionStorage = new SessionStorage(CacheService);

        TelegramBot = new TelegramBot(Config.Instance, Database, CacheService, SessionStorage);
        TelegramBot.Init();
        
        _ = TelegramBot.Listen();
        
        await RunHealthChecker();
    }

    private static async Task RunHealthChecker()
    {
        while (true)
        {
            var cache = TelegramBot.GetHealthData();
            await HealthCheckService.SaveData(cache);

            var data = HealthCheckService.GetData();

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}