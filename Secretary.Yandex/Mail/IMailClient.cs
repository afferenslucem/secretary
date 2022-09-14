namespace Secretary.Yandex.Mail;

public interface IMailClient
{
    public Task SendMail(SecretaryMailMessage message);
}