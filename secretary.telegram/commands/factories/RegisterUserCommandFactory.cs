using secretary.telegram.commands.registeruser;

namespace secretary.telegram.commands.factories;

public class RegisterUserCommandFactory: ICommandFactory
{
    public Command GetCommand()
    {
        return new RegisterUserCommand();
    }
}