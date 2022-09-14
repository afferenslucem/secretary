using Secretary.Logging;
using Secretary.Telegram;
using Serilog;

namespace Secretary.Telegram.Commands;

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