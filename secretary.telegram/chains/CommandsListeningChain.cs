﻿using secretary.telegram.commands;
using secretary.telegram.commands.factories;
using secretary.telegram.commands.registermail;
using secretary.telegram.commands.registeruser;
using secretary.telegram.commands.timeoff;

namespace secretary.telegram.chains;

public class CommandsListeningChain
{
    private CommandsChain commandsChain = new CommandsChain();

    public CommandsListeningChain()
    {
        this.commandsChain.Add(StartCommand.Key, new StartCommandFactory());
        this.commandsChain.Add(RegisterUserCommand.Key, new RegisterUserCommandFactory());
        this.commandsChain.Add(RegisterMailCommand.Key, new RegisterMailCommandFactory());
        this.commandsChain.Add(TimeOffCommand.Key, new TimeOffCommandFactory());
        this.commandsChain.Add(CancelCommand.Key, new CancelCommandFactory());
        this.commandsChain.Add(NullCommand.Key, new NullCommandFactory());
    }

    private void Add(string key, ICommandFactory commandFactory)
    {
        this.commandsChain.Add(key, commandFactory);
    }

    public Command? Get(string key)
    {
        return this.commandsChain.Get(key);
    }
}