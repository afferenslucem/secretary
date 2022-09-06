using secretary.telegram.commands.caches;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.timeoff;

public class EnterReasonCommand : Command
{
    public override Task Execute()
    {
        return Context.TelegramClient.SendMessageWithKeyBoard(ChatId, 
            "Введите причину отгула, это опционально, если вы нажмете \"Пропустить\", то этой графы не будет в заявлении.\r\n" +
            @"А если укажете, то это будет строка вида <code>Причина: {{причина}}</code>",
            new [] {"Пропустить"});
    }

    public override async Task<int> OnMessage()
    {
        if (Message == "Пропустить") return ExecuteDirection.RunNext;

        var cache = await Context.CacheService.GetEntity<TimeOffCache>(ChatId);

        if (cache == null) throw new InternalException();
        
        cache.Reason = Message;

        await Context.CacheService.SaveEntity(ChatId, cache);
        
        return ExecuteDirection.RunNext;
    }
}