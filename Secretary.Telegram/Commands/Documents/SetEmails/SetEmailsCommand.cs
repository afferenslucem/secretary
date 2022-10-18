using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.Documents.Common;

namespace Secretary.Telegram.Commands.Documents.SetEmails;

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