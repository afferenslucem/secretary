using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using MimeKit.Utils;
using Secretary.Logging;
using Secretary.Yandex.Exceptions;
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

    private string[] _commonSentNames = { "sent", "sent mails", "sent items", "отправленные" };

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
        using var message = await SendEmail(messageConfig);
            
        try
        {
            _logger.Debug("Sending message");

            await PutToSent(message);
        }
        catch (Exception e)
        {
            if (e.Message == "Could not find \"Sent\" folder")
            {
                await ForwardMessage(message, messageConfig);
                return;
            }
            
            _logger.Error(e, "Could not sent message");
            throw;
        }
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
        var sent = await GetSentSubfolderAsync(personal);
        
        _logger.Information($"Found {sent.Name} folder");

        await sent.AppendAsync(message, MessageFlags.Seen);
    }

    private async Task<IMailFolder> GetSentSubfolderAsync(IMailFolder folder)
    {
        var subfolders = await folder.GetSubfoldersAsync();
        
        foreach (var subfolder in subfolders)
        {
            if (_commonSentNames.Contains(subfolder.Name.ToLower()))
            {
                return subfolder;
            }
        }

        var subfoldersCollection = string.Join(", ", subfolders.Select(item => item.Name));
        
        _logger.Error($"Could not find \"Sent\" folder in {subfoldersCollection}");
        
        throw new YandexApiException($"Could not find \"Sent\" folder");
    }

    private async Task ForwardMessage(MimeMessage message, MailMessage messageConfig)
    {
        var sender = messageConfig.Sender.ToMailkitAddress();
        
        _logger.Debug($"Forward email to {sender}");
        
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