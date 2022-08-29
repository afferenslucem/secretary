namespace secretary.telegram.commands.factories;

public class CancelCommandFactory: ICommandFactory
{
    public Command GetCommand()
    {
        return new CancelCommand();
    }
}