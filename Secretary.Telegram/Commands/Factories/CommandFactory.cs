namespace Secretary.Telegram.Commands.Factories;

public class CommandFactory<T>: ICommandFactory
    where T: Command, new()
{
    public Command GetCommand()
    {
        return new T();
    }
}