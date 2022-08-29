namespace secretary.telegram.commands.timeoff;

public class EnterPeriodCommand : Command
{
    protected override Task ExecuteRoutine()
    {
        return Context.TelegramClient.SendMessage(ChatId, "Введите период отгула в формате <strong>DD.MM.YYYY[ с HH:mm до HH:mm]</strong>\r\n" +
                                                          "Например: <i>26.04.2020 c 9:00 до 13:00</i>\r\n" +
                                                          "Или: <i>26.04.2020</i>, если вы берете отгул на целый день\r\n" +
                                                          "В таком виде это будет вставлено в документ.\r\n\r\n" +
                                                          "Лучше соблюдать форматы даты и всемени, потому что со временем я хочу еще сделать создание события в календаре яндекса:)");
    }
    
    protected override Task OnMessageRoutine()
    {
        var parent = this.ParentCommand as TimeOffCommand;
        
        parent!.Data.Period = Message;
        
        return Task.CompletedTask;
    }
}