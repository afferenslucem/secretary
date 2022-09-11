
using secretary.logging;
using secretary.telegram.commands.exceptionHandlers;
using secretary.telegram.exceptions;
using Serilog;

namespace secretary.telegram.commands.timeoff;

public class TimeOffCommand: StatedCommand
{
    private ILogger _logger = LogPoint.GetLogger<TimeOffCommand>();
    
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
            this._logger.Error(e, $"Command was completed by exception");

            throw;
        }
    }

    private async Task HandleUserException(NonCompleteUserException e)
    {
        await new NonCompleteUserExceptionHandlerVisitor().Handle(e, ChatId, Context.TelegramClient);
    }
}