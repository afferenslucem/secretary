
using secretary.logging;
using Serilog;

namespace secretary.telegram.commands;

public class VersionCommand: Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<VersionCommand>();
    
    public const string Key = "/version";
    
    public override Task Execute()
    {
        _logger.Information($"{ChatId}: Asked version");
        
        return TelegramClient.SendMessage(TelegramBot.Version);
    }
}