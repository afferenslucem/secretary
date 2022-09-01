using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.telegram.commands.executors;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands;

public class NullCommand: Command
{
    private ILogger<NullCommand> _logger = LogPoint.GetLogger<NullCommand>();
    public const string Key = "*";

    public override async Task Execute()
    {
        var session = await Context.GetSession();

        if (session == null || session.LastCommand == null)
        {
            await Context.TelegramClient.SendMessage(ChatId, "Извините, я не понял\r\nОтправьте команду");

            return;
        }

        await new CommandExecutor(session.LastCommand, Context).OnMessage();
    }

    public override Task OnComplete()
    {
        return Task.CompletedTask;
    }
}