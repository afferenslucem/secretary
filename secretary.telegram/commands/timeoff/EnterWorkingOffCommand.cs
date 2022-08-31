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
    
    public override Task<int> OnMessage()
    {
        if (Message == "Пропустить") return Task.FromResult(RunNext);
        
        var parent = this.ParentCommand as TimeOffCommand;
        
        parent!.Data.WorkingOff = Message;
        
        return Task.FromResult(RunNext);
    }
}