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

        var bot = new TelegramBot(Config.Instance);

        await bot.Listen();
    }
}