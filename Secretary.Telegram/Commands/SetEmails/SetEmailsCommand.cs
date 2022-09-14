using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Common;

namespace Secretary.Telegram.Commands.SetEmails;

public class SetEmailsCommand : StatedCommand
{
    public const string Key = "/setemails";
    public override List<Command> ConfigureStates()
    {
        return new()
        {
            new EmptyCommand(),
            new ChooseDocumentCommand(),
            new SetReceiversCommand<SetEmailsCache>(),
            new CheckEmailsCommand<SetEmailsCache>(),
            new FinishSetEmailsCommand(),
            new AssymetricCompleteCommand(),
        };
    }
}