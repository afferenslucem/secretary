using secretary.documents.creators;
using secretary.telegram.commands.registermail;

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
    public const string Key = "/timeoff";

    public TimeOffCreateModel Data { get; set; } = new TimeOffCreateModel();
    
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
        };
    }
}