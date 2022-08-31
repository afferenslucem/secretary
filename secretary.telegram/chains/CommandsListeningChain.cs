using secretary.telegram.commands;
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
        this.commandsChain.Add(StartCommand.Key, new CommandFactory<StartCommand>());
        this.commandsChain.Add(RegisterUserCommand.Key, new CommandFactory<RegisterUserCommand>());
        this.commandsChain.Add(RegisterMailCommand.Key, new CommandFactory<RegisterMailCommand>());
        this.commandsChain.Add(TimeOffCommand.Key, new CommandFactory<TimeOffCommand>());
        this.commandsChain.Add(CancelCommand.Key, new CommandFactory<CancelCommand>());
        this.commandsChain.Add(NullCommand.Key, new CommandFactory<NullCommand>());
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