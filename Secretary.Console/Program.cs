using System.Threading.Tasks;
using Secretary.Configuration;
using Secretary.Documents;
using Secretary.Storage;
using Secretary.Telegram;

namespace Secretary.Console;

internal class Program
{
    private static async Task Main(string[] args)
    {
        DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);

        var database = new Database();
        database.MigrateDatabase();

        var bot = new TelegramBot(Config.Instance, database);
        bot.Init();

        await bot.Listen();
    }
}