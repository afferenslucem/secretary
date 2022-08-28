using secretary.storage.models;
using secretary.telegram.utils;

namespace secretary.telegram.tests.utils;

public class EmailParserTests
{
    public EmailParser parser = null!;
    
    [SetUp]
    public void Setup()
    {
        this.parser = new EmailParser();
    }

    [Test]
    public void ShouldCreate()
    {
        Assert.NotNull(parser);
    }

    [Test]
    public void ShouldParseEmailWithName()
    {
        var result = parser.Parse("a.pushkin@infinnity.ru (Александр Пушкин)");

        Assert.AreEqual(result, new Email
        {
            Address = "a.pushkin@infinnity.ru",
            DisplayName = "Александр Пушкин"
        });
    }

    [Test]
    public void ShouldParseEmailWithoutName()
    {
        var result = parser.Parse("a.pushkin@infinnity.ru");

        Assert.AreEqual(result, new Email
        {
            Address = "a.pushkin@infinnity.ru"
        });
    }

    [Test]
    public void ShouldParseEmailWithIncorrectName()
    {
        var result = parser.Parse("a.pushkin@infinnity.ru (Александр Пушкин");

        Assert.AreEqual(result, new Email
        {
            Address = "a.pushkin@infinnity.ru"
        });
    }

    [Test]
    public void ShouldThrowErrorForIncorrectEmail()
    {
        Assert.Throws<FormatException>(
            () => parser.Parse("a.pushkin@companyru (Александр Пушкин"),
            "Address \"a.pushkin@companyru (Александр Пушкин\" has invalid format"
        );
    }
    

    [Test]
    public void ShouldParseManyEmails()
    {
        var result = parser.ParseMany(
            "a.pushkin@infinnity.ru (Александр Пушкин)\n" +
            "s.esenin@infinnity.ru (Сергей Есенин)\n" +
            "v.mayakovskii@infinnity.ru");

        CollectionAssert.AreEqual(result, new Email[]
        {
            new Email { Address = "a.pushkin@infinnity.ru", DisplayName = "Александр Пушкин" },
            new Email { Address = "s.esenin@infinnity.ru", DisplayName = "Сергей Есенин" },
            new Email { Address = "v.mayakovskii@infinnity.ru"},
        });
    }
}