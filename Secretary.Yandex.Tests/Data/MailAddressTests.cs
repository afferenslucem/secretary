using Moq;
using Secretary.Yandex.Mail.Data;

namespace Secretary.Yandex.Tests.Data;

public class MailAddressTests
{
    [Test]
    public void ShouldReturnDisplayName()
    {
        var address = new MailAddress("a.pushkin@infinnity.ru", "Александр Пушкин");
        
        Assert.That(address.DisplayName, Is.EqualTo("Александр Пушкин"));
    }
    
    [Test]
    public void ShouldReturnAddressInsteadOfNullDisplayName()
    {
        var address = new MailAddress("a.pushkin@infinnity.ru", null);
        
        Assert.That(address.DisplayName, Is.EqualTo("a.pushkin@infinnity.ru"));
    }
    
    [Test]
    public void ShouldConvertToMailkitAddress()
    {
        var address = new MailAddress("a.pushkin@infinnity.ru", "Александр Пушкин");

        var result = address.ToMailkitAddress();
        
        Assert.That(result.Name, Is.EqualTo("Александр Пушкин"));
        Assert.That(result.Address, Is.EqualTo("a.pushkin@infinnity.ru"));
    }
}