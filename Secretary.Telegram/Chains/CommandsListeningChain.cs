using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Distant;
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
        _commandsChain.Add(StartCommand.Key, new CommandFactory<StartCommand>());
        _commandsChain.Add(RegisterUserCommand.Key, new CommandFactory<RegisterUserCommand>());
        _commandsChain.Add(RegisterMailCommand.Key, new CommandFactory<RegisterMailCommand>());
        _commandsChain.Add(TimeOffCommand.Key, new CommandFactory<TimeOffCommand>());
        _commandsChain.Add(VacationCommand.Key, new CommandFactory<VacationCommand>());
        _commandsChain.Add(DistantCommand.Key, new CommandFactory<DistantCommand>());
        _commandsChain.Add(CancelCommand.Key, new CommandFactory<CancelCommand>());
        _commandsChain.Add(MeCommand.Key, new CommandFactory<MeCommand>());
        _commandsChain.Add(SetEmailsCommand.Key, new CommandFactory<SetEmailsCommand>());
        _commandsChain.Add(RemindLogTimeCommand.Key, new CommandFactory<RemindLogTimeCommand>());
        _commandsChain.Add(RenewTokenCommand.Key, new CommandFactory<RenewTokenCommand>());
        _commandsChain.Add(NullCommand.Key, new CommandFactory<NullCommand>());
    }

    private void Add(string key, ICommandFactory commandFactory)
    {
        _commandsChain.Add(key, commandFactory);
    }

    public virtual Command? Get(string key)
    {
        return _commandsChain.Get(key);
    }
}