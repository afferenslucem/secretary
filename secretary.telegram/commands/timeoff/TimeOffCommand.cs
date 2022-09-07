using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.telegram.commands.caches;
using secretary.telegram.commands.exceptionHandlers;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.timeoff;

public class TimeOffCommand: StatedCommand
{
    private ILogger<TimeOffCache> _logger = LogPoint.GetLogger<TimeOffCache>();
    
    public const string Key = "/timeoff";
    
    public override List<Command> ConfigureStates()
    {
        return new List<Command>()
        {
            new EmptyCommand(), 
            new EnterPeriodCommand(),
            new EnterReasonCommand(),
            new EnterWorkingOffCommand(),
            new CheckDocumentCommand(),
            new SetEmailsCommand(),
            new CheckEmailsCommand(),
            new SendDocumentCommand(),
            new AssymetricCompleteCommand(),
        };
    }

    public override async Task Execute()
    {
        try
        {
            await base.Execute();
        }
        catch (NonCompleteUserException e)
        {
            await HandleUserException(e);
            this._logger.LogError(e, $"Command was completed by exception");

            throw;
        }
    }

    private async Task HandleUserException(NonCompleteUserException e)
    {
        await new NonCompleteUserExceptionHandlerVisitor().Handle(e, ChatId, Context.TelegramClient);
    }
}