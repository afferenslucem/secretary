using secretary.telegram.commands.executors;

namespace secretary.telegram.commands;

public class NullCommand: Command
{
    public const string Key = "*";

    public override async Task Execute()
    {
        var session = await Context.GetSession();

        if (session == null)
        {
            await TelegramClient.SendMessage("Извините, я не понял\r\nОтправьте команду");

            return;
        }

        await new CommandExecutor(session.LastCommand, Context).OnMessage();
        await new CommandExecutor(session.LastCommand, Context).OnComplete();
    }

    public override Task OnComplete()
    {
        return Task.CompletedTask;
    }
}