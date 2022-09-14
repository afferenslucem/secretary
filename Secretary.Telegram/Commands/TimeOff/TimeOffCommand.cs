using Secretary.Documents.utils;
using Secretary.Logging;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Common;
using Secretary.Telegram.Commands.ExceptionHandlers;
using Secretary.Telegram.Exceptions;
using Serilog;

namespace Secretary.Telegram.Commands.TimeOff;

public class TimeOffCommand: StatedCommand
{    
    public IFileManager FileManager { get; set; }
    
    public const string Key = "/timeoff";
    
    public TimeOffCommand(): base()
    {
        this.FileManager = new FileManager();
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
}