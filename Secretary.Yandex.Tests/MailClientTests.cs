using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using Moq;
using Secretary.Yandex.Mail;

namespace Secretary.Yandex.Tests;

public class MailClientTests
{
    private Mock<ISmtpClient> _smtpClient;
    private Mock<IImapClient> _imapClient;

    private MailClient _mailClient;
    
    [SetUp]
    public void Setup()
    {
        _smtpClient = new Mock<ISmtpClient>();
        _imapClient = new Mock<IImapClient>();

        _mailClient = new MailClient(_smtpClient.Object, _imapClient.Object);
    }

    [Test]
    public async Task ShouldConnectViaSMTP()
    {
        await _mailClient.Connect("a.pushkin@infinnity.ru", "access_token");
        
        _smtpClient.Verify(target => target.ConnectAsync("smtp.yandex.ru", 465, true, CancellationToken.None), Times.Once);
        _smtpClient.Verify(target => target.AuthenticateAsync(
            It.Is<SaslMechanismOAuth2> (
                oauth => 
                    oauth.Credentials.UserName == "a.pushkin@infinnity.ru" &&
                    oauth.Credentials.Password == "access_token")
            , CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task ShouldConnectViaIMAP()
    {
        await _mailClient.Connect("a.pushkin@infinnity.ru", "access_token");
        
        _imapClient.Verify(target => target.ConnectAsync("imap.yandex.ru", 993, true, CancellationToken.None), Times.Once);
        _imapClient.Verify(target => target.AuthenticateAsync(
            It.Is<SaslMechanismOAuth2> (
                oauth => 
                    oauth.Credentials.UserName == "a.pushkin@infinnity.ru" &&
                    oauth.Credentials.Password == "access_token")
            , CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task ShouldDisconnectViaSMTP()
    {
        await _mailClient.Disconnect();
        
        _smtpClient.Verify(target => target.DisconnectAsync(true, CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task ShouldDisconnectViaIMAP()
    {
        await _mailClient.Disconnect();
        
        _imapClient.Verify(target => target.DisconnectAsync(true, CancellationToken.None), Times.Once);
    }

    [Test]
    public void ShouldDisposeSmtpClient()
    {
        _mailClient.Dispose();
        
        _smtpClient.Verify(target => target.Dispose(), Times.Once);
    }

    [Test]
    public void ShouldDisposeImapClient()
    {
        _mailClient.Dispose();
        
        _imapClient.Verify(target => target.Dispose(), Times.Once);
    }
}