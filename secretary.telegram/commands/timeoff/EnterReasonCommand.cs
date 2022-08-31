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

    public override Task<int> OnMessage()
    {
        if (Message == "Пропустить") return Task.FromResult(RunNext);
        
        var parent = this.ParentCommand as TimeOffCommand;
        
        parent!.Data.Reason = Message;
        
        return Task.FromResult(RunNext);
    }
}