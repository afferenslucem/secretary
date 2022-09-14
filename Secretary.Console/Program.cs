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

        var database = new Database(Config.Instance.DbPath);

        database.InitDb();

        var bot = new TelegramBot(Config.Instance, database);

        await bot.Listen();
    }
}