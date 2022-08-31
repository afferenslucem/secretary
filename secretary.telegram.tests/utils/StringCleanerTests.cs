using secretary.telegram.utils;

namespace secretary.telegram.tests.utils;

public class StringCleanerTests
{
    private StringCleaner _cleaner;

    [SetUp]
    public void Setup()
    {
        this._cleaner = new StringCleaner();
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