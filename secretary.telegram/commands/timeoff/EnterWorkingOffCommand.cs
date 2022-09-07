using secretary.telegram.commands.caches;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.timeoff;

public class EnterWorkingOffCommand : Command
{
    public override Task Execute()
    {
        return TelegramClient.SendMessageWithKeyBoard( 
            "Введите данные об отработке в свободном формате.\r\n" +
            "Например: <i>Отгул обязуюсь отработать</i>\r\n" +
            "Или: Отгул <i>без отработки</i>\r\n\r\n" +
            "Если вы нажмете \"Пропустить\", то эти данные просто не будут указаны в документе.",
            new [] { "Пропустить" });
    }
    
    public override async Task<int> OnMessage()
    {
        if (Message == "Пропустить") return ExecuteDirection.RunNext;

        var cache = await CacheService.GetEntity<TimeOffCache>();

        if (cache == null) throw new InternalException();
        
        cache.WorkingOff = Message;

        await CacheService.SaveEntity(cache);
        
        return ExecuteDirection.RunNext;
    }
}