using Secretary.Telegram.Commands.Abstractions;

namespace Secretary.Telegram.Commands.Factories;

public interface ICommandFactory
{
    Command GetCommand();
}