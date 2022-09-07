using Microsoft.Extensions.Logging;
using secretary.logging;

namespace secretary.telegram.commands;

public class CancelCommand: Command
{
    private readonly ILogger<CancelCommand> _logger = LogPoint.GetLogger<CancelCommand>();
    
    public const string Key = "/cancel";
    
    public override async Task Execute()
    {
        var session = await Context.GetSession();
        
        session?.LastCommand?.Cancel();
        
        await Context.SessionStorage.DeleteSession(ChatId);
        await Context.TelegramClient.SendMessage(ChatId, "Дальнейшее выполнение команды прервано");

        var commandTypeName = session?.LastCommand?.GetType()?.Name ?? "null";
        
        _logger.LogInformation($"{ChatId}: Cancelled command {commandTypeName}");
    }

    public override Task OnComplete()
    {
        return Task.CompletedTask;
    }
}