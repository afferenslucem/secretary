using Secretary.Telegram.Utils;

namespace Secretary.Telegram.Tests.Utils;

public class StringCleanerTests
{
    private StringCleaner _cleaner = null!;

    [SetUp]
    public void Setup()
    {
        _cleaner = new StringCleaner();
    }

    [Test]
    public void ShouldTrimString()
    {
        var message = " message ";

        var result = _cleaner.Clean(message);
        
        Assert.That(result, Is.EqualTo("message"));
    }

    [Test]
    public void ShouldCleanWhitespace()
    {
        var message = "one   two  three";

        var result = _cleaner.Clean(message);
        
        Assert.That(result, Is.EqualTo("one two three"));
    }
}