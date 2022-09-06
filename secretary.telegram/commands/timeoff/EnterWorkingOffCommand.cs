using secretary.telegram.commands.caches;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.timeoff;

public class EnterWorkingOffCommand : Command
{
    public override Task Execute()
    {
        return Context.TelegramClient.SendMessageWithKeyBoard(ChatId, 
            "Введите данные об отработке в свободном формате.\r\n" +
            "Например: <i>Отгул обязуюсь отработать</i>\r\n" +
            "Или: Отгул <i>без отработки</i>\r\n\r\n" +
            "Если вы нажмете \"Пропустить\", то эти данные просто не будут указаны в документе.",
            new [] { "Пропустить" });
    }
    
    public override async Task<int> OnMessage()
    {
        if (Message == "Пропустить") return ExecuteDirection.RunNext;

        var cache = await Context.CacheService.GetEntity<TimeOffCache>(ChatId);

        if (cache == null) throw new InternalException();
        
        cache.WorkingOff = Message;

        await Context.CacheService.SaveEntity(ChatId, cache);
        
        return ExecuteDirection.RunNext;
    }
}