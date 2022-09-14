using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Caches.Interfaces;
using Secretary.Telegram.Commands.TimeOff;
using Secretary.Telegram.Exceptions;

namespace Secretary.Telegram.Commands.Common;

public class CheckEmailsCommand<T> : Command
    where T: class, IEmailsCache, IDocumentKeyCache
{
    public override async Task Execute()
    {
        var cache = await CacheService.GetEntity<T>();
        if (cache?.Emails == null) throw new InternalException();
        
        var emailsPrints = cache.Emails
            .Select(item => item.DisplayName != null ? $"{item.Address} ({item.DisplayName})" : item.Address);

        var emailTable = string.Join("\n", emailsPrints);

        var message = "Заявление будет отправлено на следующие адреса:\n" +
                      "<code>\n" +
                      $"{emailTable}" +
                      "</code>\n" +
                      "\n" +
                      "Все верно?";
        
        await TelegramClient.SendMessageWithKeyBoard(message, new [] { "Верно", "Нет, нужно поправить" });
    }

    public override async Task<int> OnMessage()
    {
        if (Message.ToLower() == "верно")
        {
            var cache = await CacheService.GetEntity<T>();
            if (cache?.Emails == null) throw new InternalException();

            var document = await DocumentStorage.GetOrCreateDocument(cache.DocumentKey);
            await EmailStorage.SaveForDocument(document.Id, cache.Emails);
            
            return ExecuteDirection.RunNext;
        }
        else
        {
            return ExecuteDirection.GoBack;
        }
    }
}