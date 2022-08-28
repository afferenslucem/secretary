namespace secretary.yandex.mail;

public interface IMailClient
{
    public Task SendMail(SecretaryMailMessage message);
}