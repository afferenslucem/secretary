using Secretary.Telegram.Commands.Executors;

namespace Secretary.Telegram.Commands;

public class NullCommand: Command
{
    public const string Key = "*";

    public override async Task Execute()
    {
        var session = await SessionStorage.GetSession();

        if (session == null)
        {
            await TelegramClient.SendMessage("Извините, я не понял\nОтправьте команду");

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