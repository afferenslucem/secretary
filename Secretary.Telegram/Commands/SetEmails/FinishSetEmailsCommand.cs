namespace Secretary.Telegram.Commands.SetEmails;

public class FinishSetEmailsCommand : Command
{
    public override Task Execute()
    {
        return TelegramClient.SendMessage("Адресаты успешно изменены");
    }
}