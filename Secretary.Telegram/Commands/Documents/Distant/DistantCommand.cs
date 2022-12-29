using Secretary.Documents.utils;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.Documents.Common;

namespace Secretary.Telegram.Commands.Documents.Distant;

public class DistantCommand: StatedCommand
{    
    public IFileManager FileManager { get; set; }
    
    public const string Key = "/distant";
    
    public DistantCommand()
    {
        FileManager = new FileManager();
    }
    
    public override List<Command> ConfigureStates()
    {
        return new List<Command>()
        {
            new EmptyCommand(), 
            new EnterDistantPeriodCommand<DistantCache>(),
            new EnterDistantReasonCommand<DistantCache>(),
            new CheckDocumentCommand<DistantCache>(),
            new SetReceiversCommand<DistantCache>(),
            new CheckEmailsCommand<DistantCache>(),
            new SendDocumentCommand<DistantCache>(),
            new AssymetricCompleteCommand(),
        };
    }

    public override async Task OnForceComplete()
    {
        var cache = await CacheService.GetEntity<DistantCache>();

        FileManager.DeleteFile(cache?.FilePath);
        
        await CacheService.DeleteEntity<DistantCache>();
        await base.OnForceComplete();
    }
    
    public override async Task OnComplete()
    {
        await base.OnComplete();
        
        if (IsCompleted)
        {
            await StatisticService.LogDistant(ChatId);
        }
    }
}