namespace secretary.telegram.commands.registermail
;

public class RegisterMailCommand: StatedCommand
{
    public const string Key = "/registermail";
    
    public override List<Command> ConfigureStates()
    {
        return new List<Command>
        {
            new EmptyCommand(),
            new EnterEmailCommand(),
            new EnterCodeCommand(),
            new AssymetricCompleteCommand(),
        };
    }
}