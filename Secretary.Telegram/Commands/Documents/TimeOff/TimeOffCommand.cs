using Secretary.Documents.utils;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.Documents.Common;

namespace Secretary.Telegram.Commands.Documents.TimeOff;

public class TimeOffCommand: StatedCommand
{    
    public IFileManager FileManager { get; set; }
    
    public const string Key = "/timeoff";
    
    public TimeOffCommand()
    {
        FileManager = new FileManager();
    }
    
    public override List<Command> ConfigureStates()
    {
        return new List<Command>()
        {
            new EmptyCommand(), 
            new EnterTimeOffPeriodCommand<TimeOffCache>(),
            new EnterReasonCommand<TimeOffCache>(),
            new EnterWorkingOffCommand<TimeOffCache>(),
            new CheckDocumentCommand<TimeOffCache>(),
            new SetReceiversCommand<TimeOffCache>(),
            new CheckEmailsCommand<TimeOffCache>(),
            new SendDocumentCommand<TimeOffCache>(),
            new AssymetricCompleteCommand(),
        };
    }

    public override async Task OnForceComplete()
    {
        var cache = await CacheService.GetEntity<TimeOffCache>();

        FileManager.DeleteFile(cache?.FilePath);
        
        await CacheService.DeleteEntity<TimeOffCache>();
        await base.OnForceComplete();
    }

    public override async Task OnComplete()
    {
        await base.OnComplete();
        
        if (IsCompleted)
        {
            await StatisticService.LogTimeOff(ChatId);
        }
    }
}