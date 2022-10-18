using Secretary.Telegram.Commands.Abstractions;

namespace Secretary.Telegram.Commands.Jira.LogTime;

public class LogTimeCommand: StatedCommand
{
    public const string Key = "/logtime";
    
    public override List<Command> ConfigureStates()
    {
        return new List<Command>()
        {
            new EmptyCommand(),
            new LogTimeActionCommand()
        };
    }
}