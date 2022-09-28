using Secretary.Yandex.Mail.Data;

namespace Secretary.Yandex.Mail;

public interface IMailSender
{
    Task SendMail(MailMessage message);
}