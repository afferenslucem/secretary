namespace secretary.telegram.commands.factories;

public class RegisterMailCommandFactory: ICommandFactory
{
    public Command GetCommand()
    {
        return new RegisterMailCommand();
    }
}