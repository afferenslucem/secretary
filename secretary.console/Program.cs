using System.Threading.Tasks;
using secretary.configuration;
using secretary.documents;
using secretary.storage;
using secretary.telegram;

namespace secretary.console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);

            var database = new Database(Config.Instance.DbPath);
            
            database.InitDb();

            var bot = new TelegramBot(Config.Instance, database);

            await bot.Listen();
        }
    }
}