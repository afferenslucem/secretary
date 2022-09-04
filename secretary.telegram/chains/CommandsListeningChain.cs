using secretary.telegram.commands;
using secretary.telegram.commands.factories;
using secretary.telegram.commands.registermail;
using secretary.telegram.commands.registeruser;
using secretary.telegram.commands.timeoff;

namespace secretary.telegram.chains;

public class CommandsListeningChain
{
    private readonly CommandsChain _commandsChain = new();

    public CommandsListeningChain()
    {
        this._commandsChain.Add(StartCommand.Key, new CommandFactory<StartCommand>());
        this._commandsChain.Add(RegisterUserCommand.Key, new CommandFactory<RegisterUserCommand>());
        this._commandsChain.Add(RegisterMailCommand.Key, new CommandFactory<RegisterMailCommand>());
        this._commandsChain.Add(TimeOffCommand.Key, new CommandFactory<TimeOffCommand>());
        this._commandsChain.Add(CancelCommand.Key, new CommandFactory<CancelCommand>());
        this._commandsChain.Add(MeCommand.Key, new CommandFactory<MeCommand>());
        this._commandsChain.Add(VersionCommand.Key, new CommandFactory<VersionCommand>());
        this._commandsChain.Add(NullCommand.Key, new CommandFactory<NullCommand>());
    }

    private void Add(string key, ICommandFactory commandFactory)
    {
        this._commandsChain.Add(key, commandFactory);
    }

    public Command? Get(string key)
    {
        return this._commandsChain.Get(key);
    }
}