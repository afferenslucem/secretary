using Secretary.Documents.utils;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.Documents.Common;

namespace Secretary.Telegram.Commands.Documents.Vacation;

public class VacationCommand: StatedCommand
{
    public IFileManager FileManager { get; set; }
    
    public const string Key = "/vacation";

    public VacationCommand()
    {
        FileManager = new FileManager();
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
    
    public override async Task OnComplete()
    {
        await base.OnComplete();
        
        if (IsCompleted)
        {
            await StatisticService.LogVacation(ChatId);
        }
    }
}