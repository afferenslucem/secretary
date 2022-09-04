using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.storage.models;

namespace secretary.telegram.commands;

public class VersionCommand: Command
{
    private readonly ILogger<VersionCommand> _logger = LogPoint.GetLogger<VersionCommand>();
    
    public const string Key = "/version";
    
    public override Task Execute()
    {
        _logger.LogInformation($"{ChatId}: Asked version");
        
        return Context.TelegramClient.SendMessage(ChatId, TelegramBot.Version);
    }
}