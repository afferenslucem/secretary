using secretary.telegram.commands.timeoff;

namespace secretary.telegram.commands.factories;

public class TimeOffCommandFactory: ICommandFactory
{
    public Command GetCommand()
    {
        return new TimeOffCommand();
    }
}