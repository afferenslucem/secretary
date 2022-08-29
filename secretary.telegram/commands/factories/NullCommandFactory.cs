namespace secretary.telegram.commands.factories;

public class NullCommandFactory: ICommandFactory
{
    public Command GetCommand()
    {
        return new NullCommand();
    }
}