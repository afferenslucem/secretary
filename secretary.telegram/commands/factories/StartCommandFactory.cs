namespace secretary.telegram.commands.factories;

public class StartCommandFactory: ICommandFactory
{
    public Command GetCommand()
    {
        return new StartCommand();
    }
}