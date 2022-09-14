using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Caches.Interfaces;
using Secretary.Telegram.Exceptions;

namespace Secretary.Telegram.Commands.Common;

public class EnterReasonCommand<T> : Command
    where T : class, IReasonCache
{
    public override Task Execute()
    {
        return TelegramClient.SendMessageWithKeyBoard( 
            "Введите причину отгула, это опционально, если вы нажмете \"Пропустить\", то этой графы не будет в заявлении.\n" +
            @"А если укажете, то это будет строка вида <code>Причина: {{причина}}</code>",
            new [] {"Пропустить"});
    }

    public override async Task<int> OnMessage()
    {
        if (Message == "Пропустить") return ExecuteDirection.RunNext;

        var cache = await CacheService.GetEntity<T>();

        if (cache == null) throw new InternalException();
        
        cache.Reason = Message;

        await CacheService.SaveEntity(cache);
        
        return ExecuteDirection.RunNext;
    }
}