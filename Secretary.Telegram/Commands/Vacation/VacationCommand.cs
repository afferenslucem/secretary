using Secretary.Documents.utils;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Common;

namespace Secretary.Telegram.Commands.Vacation;

public class VacationCommand: StatedCommand
{
    public IFileManager FileManager { get; set; }
    
    public const string Key = "/vacation";

    public VacationCommand(): base()
    {
        this.FileManager = new FileManager();
    }
    
    public override List<Command> ConfigureStates()
    {
        return new()
        {
            new EmptyCommand(),
            new EnterVacationPeriodCommand<VacationCache>(),
            new CheckDocumentCommand<VacationCache>(),
            new SetReceiversCommand<VacationCache>(),
            new CheckEmailsCommand<VacationCache>(),
            new SendDocumentCommand<VacationCache>(),
            new AssymetricCompleteCommand(),
        };
    }

    public override async Task OnForceComplete()
    {
        var cache = await CacheService.GetEntity<VacationCache>();

        FileManager.DeleteFile(cache?.FilePath);
        
        await CacheService.DeleteEntity<VacationCache>();
        await base.OnForceComplete();
    }
}