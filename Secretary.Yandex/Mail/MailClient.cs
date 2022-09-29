using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using MimeKit.Utils;
using Secretary.Logging;
using Secretary.Yandex.Mail.Data;
using Serilog;

namespace Secretary.Yandex.Mail;

public class MailClient: IMailClient
{
    private ILogger _logger = LogPoint.GetLogger<MailClient>();
    
    private string _user;
    private string _token;

    private ISmtpClient _smtpClient;
    private IImapClient _imapClient;

    public MailClient()
    {
        _smtpClient = new SmtpClient();
        _imapClient = new ImapClient();
    }

    public MailClient(ISmtpClient smtpClient, IImapClient imapClient)
    {
        _smtpClient = smtpClient;
        _imapClient = imapClient;
    }
    
    public async Task Connect(string user, string token)
    {
        _user = user;
        _token = token;
        
        await Task.WhenAll(this.ConnectToSMTP(), this.ConnectToIMAP());
    }

    private async Task ConnectToSMTP()
    {
        var host = "smtp.yandex.ru";
        var port = 465;
        
        _logger.Debug($"Connecting to {host}:{port}");

        await _smtpClient.ConnectAsync(host, port, true);
        
        _logger.Debug($"Authenticating at {host} like {_user}");
        var oauth2 = new SaslMechanismOAuth2(_user, _token);

        await _smtpClient.AuthenticateAsync(oauth2);
        
        _logger.Debug($"Authenticated at {host}");
    }

    private async Task ConnectToIMAP()
    {
        var host = "imap.yandex.ru";
        var port = 993;
        
        _logger.Debug($"Connecting to {host}:{port}");

        await _imapClient.ConnectAsync(host, port, true);
        
        _logger.Debug($"Authenticating at {host} like {_user}");
        var oauth2 = new SaslMechanismOAuth2(_user, _token);

        await _imapClient.AuthenticateAsync(oauth2);
        
        _logger.Debug($"Authenticated at {host}");
    }
    
    public async Task Disconnect()
    {
        _logger.Debug("Disconnecting");
        await Task.WhenAll(_imapClient.DisconnectAsync(true), _smtpClient.DisconnectAsync(true));
    }

    public async Task SendMail(MailMessage messageConfig)
    {
        _logger.Debug("Sending message");

        using var message = await SendEmail(messageConfig);
        await PutToSent(message);
    }

    private async Task<MimeMessage> SendEmail(MailMessage messageConfig)
    {
        var receivers = messageConfig.Receivers
            .Select(item => item.ToMailkitAddress())
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
        
        var result = new MimeMessage(
            new [] { new MailboxAddress(messageConfig.Sender.DisplayName, messageConfig.Sender.Address) },
            receivers,
            messageConfig.Theme,
            multipart
        );
        
        await _smtpClient.SendAsync(result);

        return result;
    }
    
    private async Task PutToSent(MimeMessage message)
    {
        var personal = _imapClient.GetFolder(_imapClient.PersonalNamespaces[0]);
        var sent = await personal.GetSubfolderAsync("Sent");

        await sent.AppendAsync(message, MessageFlags.Seen);

        await _imapClient.DisconnectAsync(true).ConfigureAwait(false);
    }

    private async Task ForwardMessage(MimeMessage message, MailMessage messageConfig)
    {
        var sender = messageConfig.Sender.ToMailkitAddress();
        
        message.ResentFrom.Add (sender);
        message.ResentTo.Add (sender);
        message.ResentMessageId = MimeUtils.GenerateMessageId ();
        message.ResentDate = DateTimeOffset.Now;
        
        await _smtpClient.SendAsync(message);
    }

    public void Dispose()
    {
        _smtpClient.Dispose();
        _imapClient.Dispose();
    }
}