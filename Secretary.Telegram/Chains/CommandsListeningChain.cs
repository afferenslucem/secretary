using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Factories;
using Secretary.Telegram.Commands.RegisterMail;
using Secretary.Telegram.Commands.RegisterUser;
using Secretary.Telegram.Commands.SetEmails;
using Secretary.Telegram.Commands.TimeOff;
using Secretary.Telegram.Commands.Vacation;

namespace Secretary.Telegram.chains;

public class CommandsListeningChain
{
    private readonly CommandsChain _commandsChain = new();

    public CommandsListeningChain()
    {
        this._commandsChain.Add(StartCommand.Key, new CommandFactory<StartCommand>());
        this._commandsChain.Add(RegisterUserCommand.Key, new CommandFactory<RegisterUserCommand>());
        this._commandsChain.Add(RegisterMailCommand.Key, new CommandFactory<RegisterMailCommand>());
        this._commandsChain.Add(TimeOffCommand.Key, new CommandFactory<TimeOffCommand>());
        this._commandsChain.Add(VacationCommand.Key, new CommandFactory<VacationCommand>());
        this._commandsChain.Add(CancelCommand.Key, new CommandFactory<CancelCommand>());
        this._commandsChain.Add(MeCommand.Key, new CommandFactory<MeCommand>());
        this._commandsChain.Add(SetEmailsCommand.Key, new CommandFactory<SetEmailsCommand>());
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