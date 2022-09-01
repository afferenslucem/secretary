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
        try
        {
            var session = await Context.GetSession();

            if (session == null || session.LastCommand == null)
            {
                await Context.TelegramClient.SendMessage(ChatId, "Извините, я не понял\r\nОтправьте команду");

                return;
            }

            ;

            await new CommandExecutor(session.LastCommand, Context).OnMessage();
            await new CommandExecutor(session.LastCommand, Context).OnComplete();
        }
        catch (ForceCompleteCommandException e)
        {
            await Context.SessionStorage.DeleteSession(ChatId);
            _logger.LogWarning($"Сommand {e.CommandName} force completed");
        }
    }

    public override Task OnComplete()
    {
        return Task.CompletedTask;
    }
}