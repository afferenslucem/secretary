using Secretary.Telegram.Commands.Abstractions;

namespace Secretary.Telegram.Commands.Documents.SetEmails;

public class FinishSetEmailsCommand : Command
{
    public override Task Execute()
    {
        return TelegramClient.SendMessage("Адресаты успешно изменены");
    }
}