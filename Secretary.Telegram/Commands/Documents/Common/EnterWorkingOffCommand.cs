using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Documents.Interfaces;
using Secretary.Telegram.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Commands.Documents.Common;

public class EnterWorkingOffCommand<T> : Command
    where T: class, IWorkingOffCache
{
    public override Task Execute()
    {
        return TelegramClient.SendMessage(
            "Введите данные об отработке в свободном формате.\n" +
            "Например: <i>Отгул обязуюсь отработать</i>\n" +
            "Или: Отгул <i>без отработки</i>\n\n" +
            "Если вы нажмете \"Пропустить\", то эти данные просто не будут указаны в документе.",
            (ReplyKeyboardMarkup) "Пропустить"!
        );
    }
    
    public override async Task<int> OnMessage()
    {
        if (Message == "Пропустить") return ExecuteDirection.RunNext;

        var cache = await CacheService.GetEntity<T>();

        if (cache == null) throw new InternalException();
        
        cache.WorkingOff = Message;

        await CacheService.SaveEntity(cache);
        
        return ExecuteDirection.RunNext;
    }
}