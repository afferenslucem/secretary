using Secretary.Logging;
using Secretary.Telegram.Commands.Executors;
using Serilog;

namespace Secretary.Telegram.Commands;

public class CancelCommand: Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<CancelCommand>();
    
    public const string Key = "/cancel";
    
    public override async Task Execute()
    {
        var session = await SessionStorage.GetSession();

        if (session == null)
        {
            _logger.Warning($"{ChatId}: Cancelled requested for empty session");
            return;
        };

        await new CommandExecutor(session.LastCommand, Context).Cancel();

        await TelegramClient.SendMessage("Дальнейшее выполнение команды прервано");


        await new CommandExecutor(session.LastCommand, Context).OnForceComplete();
        
        var commandTypeName = session?.LastCommand.GetType()?.Name ?? "null";
        _logger.Information($"{ChatId}: Cancelled command {commandTypeName}");
    }

    public override Task OnComplete()
    {
        return Task.CompletedTask;
    }
}