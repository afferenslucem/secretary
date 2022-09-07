using secretary.telegram.commands.caches;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.timeoff;

public class CheckEmailsCommand : Command
{
    public override async Task Execute()
    {
        var cache = await Context.CacheService.GetEntity<TimeOffCache>(ChatId);
        if (cache?.Emails == null) throw new InternalException();
        
        var emailsPrints = cache.Emails
            .Select(item => item.DisplayName != null ? $"{item.Address} ({item.DisplayName})" : item.Address);

        var emailTable = string.Join("\r\n", emailsPrints);

        var message = "Заявление будет отправлено на следующие адреса:\r\n" +
                      "<code>\r\n" +
                      $"{emailTable}" +
                      "</code>\r\n" +
                      "\r\n" +
                      "Все верно?";
        
        await TelegramClient.SendMessageWithKeyBoard(message, new [] { "Верно", "Нет, нужно поправить" });
    }

    public override async Task<int> OnMessage()
    {
        if (Message.ToLower() == "верно")
        {
            var cache = await Context.CacheService.GetEntity<TimeOffCache>(ChatId);
            if (cache?.Emails == null) throw new InternalException();

            var document = await DocumentStorage.GetOrCreateDocument(TimeOffCommand.Key);
            await Context.EmailStorage.SaveForDocument(document.Id, cache.Emails);
            
            return ExecuteDirection.RunNext;
        }
        else
        {
            return ExecuteDirection.GoBack;
        }
    }
}