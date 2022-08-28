namespace secretary.telegram.commands;

public class NullCommand: Command
{
    public const string Key = "*";
    
    protected override async Task ExecuteRoutine()
    {
        var session = await Context.GetSession();

        if (session == null || session.LastCommand == null)
        {
            await Context.TelegramClient.SendMessage(ChatId, "Извините, я не понял\r\nОтправьте команду");
            
            return;
        };
        
        await session.LastCommand.OnMessage(Context);
    }
}