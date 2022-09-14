namespace Secretary.Telegram.Commands.RegisterUser;

public class RegisterUserCommand: StatedCommand
{
    public const string Key = "/registeruser";
    
    public override List<Command> ConfigureStates()
    {
        return new List<Command>
        {
            new EmptyCommand(),
            new EnterNameCommand(),
            new EnterNameGenitiveCommand(),
            new EnterJobTitleCommand(),
            new EnterJobTitleGenitiveCommand(),
        };
    }
}