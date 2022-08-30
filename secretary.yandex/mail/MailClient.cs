using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace secretary.yandex.mail;

public class MailClient: IMailClient
{
    public async Task SendMail(SecretaryMailMessage messageConfig)
    {
        using var client = new SmtpClient();
        
        await client.ConnectAsync("smtp.yandex.ru", 465, true);
        
        var oauth2 = new SaslMechanismOAuth2(messageConfig.Sender.Address, messageConfig.Token);
        await client.AuthenticateAsync(oauth2);

        var receivers = messageConfig.Receivers
            .Select(item => new MailboxAddress(item.DisplayName ?? item.Address, item.Address))
            .ToArray();

        var html = new TextPart(TextFormat.Html);
        html.Text = messageConfig.HtmlBody;

        var attachments = messageConfig.Attachments.Select(item => new MimePart(item.ContentType)
        {
            Content = new MimeContent(File.OpenRead(item.Path)),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = item.FileName
        });
        
        var multipart = new Multipart("mixed");
        
        multipart.Add(html);

        foreach (var attachment in attachments)
        {
            multipart.Add(attachment);
        }
        
        var message = new MimeMessage(
            new [] { new MailboxAddress(messageConfig.Sender.DisplayName, messageConfig.Sender.Address) },
            receivers,
            messageConfig.Theme,
            multipart
        );

        await client.SendAsync(message);

        await client.DisconnectAsync(true);
    }
}