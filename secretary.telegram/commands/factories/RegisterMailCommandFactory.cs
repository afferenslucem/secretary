﻿using secretary.telegram.commands.registermail;

namespace secretary.telegram.commands.factories;

public class RegisterMailCommandFactory: ICommandFactory
{
    public Command GetCommand()
    {
        return new RegisterMailCommand();
    }
}