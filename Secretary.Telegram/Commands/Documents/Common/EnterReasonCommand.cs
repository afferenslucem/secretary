using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Documents.Interfaces;
using Secretary.Telegram.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Commands.Documents.Common;

public class EnterReasonCommand<T> : Command
    where T : class, IReasonCache
{
    public override Task Execute()
    {
        return TelegramClient.SendMessage( 
            "Введите причину, это опционально, если вы нажмете \"Пропустить\", то этой графы не будет в заявлении.\n" +
            "А если укажете, то это будет строка вида\n<code>Причина: {{причина}}</code>",
            (ReplyKeyboardMarkup)"Пропустить"!
        );
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