using secretary.telegram.commands.caches;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.registeruser;

public class EnterNameGenitiveCommand : Command
{
    public override Task Execute()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите ваши ФИО в родительном падеже.\r\n" +
                                                          "Так они будут указаны в отправляемом документе в графе \"от кого\".\r\n" +
                                                          @"Например: От <i>Пушкина Александра Сергеевича</i>");
    }

    public override async Task<int> OnMessage()
    {
        var cache = await Context.CacheService.GetEntity<RegisterUserCache>(ChatId);
        if (cache == null) throw new InternalException();
        
        cache.NameGenitive = Message;

        await Context.CacheService.SaveEntity(ChatId, cache);
        
        return RunNext;
    }
}