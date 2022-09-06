using secretary.telegram.commands.caches;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.registeruser;

public class EnterJobTitleCommand : Command
{
    public override Task Execute()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите вашу должность в именительном падеже.\r\n" +
                                                          "Так она будут указана в подписи письма.\r\n" +
                                                          @"Например: С уважением, <i>поэт</i> Александр Пушкин");
    }

    public override async Task<int> OnMessage()
    {
        var cache = await Context.CacheService.GetEntity<RegisterUserCache>(ChatId);
        if (cache == null) throw new InternalException();
        
        cache.JobTitle = Message;

        await Context.CacheService.SaveEntity(ChatId, cache);
        
        return ExecuteDirection.RunNext;
    }
}