using Secretary.Telegram.Commands.Abstractions;

namespace Secretary.Telegram.Commands.Jira.RegisterJiraToken;

public class RegisterJiraTokenCommand: StatedCommand
{
    public const string Key = "/registerjiratoken";


    public override List<Command> ConfigureStates()
    {
        return new List<Command>()
        {
            new EmptyCommand(),
            new RegisterJiraTokenActionCommand()
        };
    }
}