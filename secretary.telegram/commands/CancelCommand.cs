namespace secretary.telegram.commands;

public class CancelCommand: Command
{
    public const string Key = "/cancel";
    
    protected override async Task ExecuteRoutine()
    {
        await Context.SessionStorage.DeleteSession(ChatId);
        await Context.TelegramClient.SendMessage(ChatId, "Дальнейшее выполнение команды прервано");
    }
}