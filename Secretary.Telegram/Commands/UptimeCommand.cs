using Secretary.Logging;
using Secretary.Telegram;
using Serilog;

namespace Secretary.Telegram.Commands;

public class UptimeCommand: Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<UptimeCommand>();
    
    public const string Key = "/uptime";
    
    public override Task Execute()
    {
        _logger.Information($"{ChatId}: Asked uptime");
        
        return TelegramClient.SendMessage(TelegramBot.Uptime.ToString("yyyy-MM-dd HH:mm:ss zz"));
    }
}