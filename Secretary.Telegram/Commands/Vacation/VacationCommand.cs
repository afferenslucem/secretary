using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Common;

namespace Secretary.Telegram.Commands.Vacation;

public class VacationCommand: StatedCommand
{
    public const string Key = "/vacation";
    
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
}