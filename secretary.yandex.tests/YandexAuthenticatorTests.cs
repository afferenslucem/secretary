using secretary.configuration;
using secretary.yandex.authentication;

namespace secretary.yandex.tests;

public class YandexAuthenticatorTests
{
    private IYandexAuthenticator _authenticator = null!;
    private MailConfig _config = null!;
    
    [SetUp]
    public void Setup()
    {
        _config = new MailConfig();
        _authenticator = new YandexAuthenticator(_config);
    }

    [Test]
    public void ShouldAllowAllowedDomain()
    {
        _config.AllowedSenderDomains = new[] { "infinnity.ru", "gmail.com" };

        var result = _authenticator.IsUserDomainAllowed("a.pushkin@infinnity.ru");
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void ShouldNotAllowUserDomain()
    {
        _config.AllowedSenderDomains = new[] { "infinnity.ru", "gmail.com" };

        var result = _authenticator.IsUserDomainAllowed("a.pushkin@yandex.ru");
        
        Assert.That(result, Is.False);
    }
}