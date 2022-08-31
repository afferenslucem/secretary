using secretary.telegram.commands.timeoff;

namespace secretary.telegram.commands.factories;

public class CommandFactory<T>: ICommandFactory
    where T: Command, new()
{
    public Command GetCommand()
    {
        return new T();
    }
}