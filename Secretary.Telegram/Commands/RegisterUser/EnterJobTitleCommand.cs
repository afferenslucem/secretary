using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Exceptions;

namespace Secretary.Telegram.Commands.RegisterUser;

public class EnterJobTitleCommand : Command
{
    public override Task Execute()
    {
        return TelegramClient.SendMessage("Введите вашу должность в именительном падеже.\n" +
                                          "Так она будут указана в подписи письма.\n" +
                                          @"Например: С уважением, <i>поэт</i> Александр Пушкин");
    }

    public override async Task<int> OnMessage()
    {
        var cache = await CacheService.GetEntity<RegisterUserCache>();
        if (cache == null) throw new InternalException();
        
        cache.JobTitle = Message;

        await CacheService.SaveEntity(cache);
        
        return ExecuteDirection.RunNext;
    }
}