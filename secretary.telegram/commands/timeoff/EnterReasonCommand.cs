namespace secretary.telegram.commands.timeoff;

public class EnterReasonCommand : Command
{
    protected override Task ExecuteRoutine()
    {
        return Context.TelegramClient.SendMessageWithKeyBoard(ChatId, 
            "Введите причину отгула, это опционально, если вы нажмете \"Пропустить\", то этой графы не будет в заявлении.\r\n" +
            @"А если укажете, то это будет строка вида <code>Причина: {{причина}}</code>",
            new [] {"Пропустить"});
    }

    protected override Task OnMessageRoutine()
    {
        if (Message == "Пропустить") return Task.CompletedTask;
        
        var parent = this.ParentCommand as TimeOffCommand;
        
        parent!.Data.Reason = Message;
        
        return Task.CompletedTask;
    }
}