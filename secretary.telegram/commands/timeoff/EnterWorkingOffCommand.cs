namespace secretary.telegram.commands.timeoff;

public class EnterWorkingOffCommand : Command
{
    protected override Task ExecuteRoutine()
    {
        return Context.TelegramClient.SendMessageWithKeyBoard(ChatId, 
            "Введите данные об отработке в свободном формате.\r\n" +
            "Например: <i>Отгул обязуюсь отработать</i>\r\n" +
            "Или: Отгул <i>без отработки</i>\r\n\r\n" +
            "Если вы нажмете \"Пропустить\", то эти данные просто не будут указаны в документе.",
            new [] { "Пропустить" });
    }
    
    protected override Task OnMessageRoutine()
    {
        if (Message == "Пропустить") return Task.CompletedTask;
        
        var parent = this.ParentCommand as TimeOffCommand;
        
        parent!.Data.WorkingOff = Message;
        
        return Task.CompletedTask;
    }
}