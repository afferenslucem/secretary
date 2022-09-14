using Secretary.Logging;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Common;
using Secretary.Telegram.Commands.ExceptionHandlers;
using Secretary.Telegram.Exceptions;
using Serilog;

namespace Secretary.Telegram.Commands.TimeOff;

public class TimeOffCommand: StatedCommand
{
    public const string Key = "/timeoff";
    
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
}