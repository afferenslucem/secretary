using Secretary.Yandex.Mail.Data;

namespace Secretary.Yandex.Mail;

public interface IMailClient: IDisposable
{
    public Task SendMail(MailMessage message);
    Task Connect(string user, string token);

    Task Disconnect();
}