using System.Threading.Tasks;
using secretary.configuration;
using secretary.documents;
using secretary.telegram;
using secretary.storage;

namespace secretary.console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);

            var database = new Database(Config.Instance.DbPath);
            
            database.InitDb();

            var bot = new TelegramBot(Config.Instance.TelegramApiKey, Config.Instance.MailConfig, database);

            await bot.Listen();
        }
    }
}