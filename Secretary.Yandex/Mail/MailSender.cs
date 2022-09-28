using Secretary.Yandex.Mail.Data;

namespace Secretary.Yandex.Mail;

public class MailSender: IMailSender
{
    public async Task SendMail(MailMessage message)
    {
        using var mailClient = new MailClient();

        await mailClient.Connect(message.Sender.Address, message.Token);
            
        await mailClient.SendMail(message);

        await mailClient.Disconnect();
    }
}