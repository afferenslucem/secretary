using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Documents.Interfaces;
using Secretary.Telegram.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Commands.Documents.Distant;

public class EnterDistantReasonCommand<T> : Command
    where T : class, IReasonCache
{
    public override Task Execute()
    {
        return TelegramClient.SendMessage( 
            "Введите причину перехода на удаленную работу\n" +
            "В документ это будет вставлено как строка вида\n<code>Причина: {{причина}}</code>"
        );
    }

    public override async Task<int> OnMessage()
    {
        var cache = await CacheService.GetEntity<T>();

        if (cache == null) throw new InternalException();
        
        cache.Reason = Message;

        await CacheService.SaveEntity(cache);
        
        return ExecuteDirection.RunNext;
    }
}