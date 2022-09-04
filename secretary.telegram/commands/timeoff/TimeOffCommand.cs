using Microsoft.Extensions.Logging;
using secretary.documents.creators;
using secretary.logging;
using secretary.telegram.commands.exceptionHandlers;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.timeoff;

public class TimeOffCreateModel 
{
    public string? Period { get; set; }
    public string? Reason { get; set; }
    public string? WorkingOff { get; set; }
    
    public string? FilePath { get; set; }

    public TimeOffData ToDocumentData()
    {
        return new TimeOffData()
        {
            Period = this.Period,
            Reason = this.Reason,
            WorkingOff = this.WorkingOff,
        };
    }
}

public class TimeOffCommand: StatedCommand
{
    private ILogger<TimeOffCreateModel> _logger = LogPoint.GetLogger<TimeOffCreateModel>();
    
    public const string Key = "/timeoff";

    public TimeOffCreateModel Data { get; set; } = new ();
    
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
            await OnComplete();
            
            this._logger.LogError(e, $"Command was completed by exception");
        }
    }

    private async Task HandleUserException(NonCompleteUserException e)
    {
        await new NonCompleteUserExceptionHandlerVisitor().Handle(e, ChatId, Context.TelegramClient);
    }
}