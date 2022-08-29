using secretary.storage.models;
using secretary.telegram.utils;

namespace secretary.telegram.tests.utils;

public class EmailParserTests
{
    public EmailParser Parser = null!;
    
    [SetUp]
    public void Setup()
    {
        this.Parser = new EmailParser();
    }

    [Test]
    public void ShouldCreate()
    {
        Assert.NotNull(Parser);
    }

    [Test]
    public void ShouldParseEmailWithName()
    {
        var result = Parser.Parse("a.pushkin@infinnity.ru (Александр Пушкин)");

        Assert.That(new Email
        {
            Address = "a.pushkin@infinnity.ru",
            DisplayName = "Александр Пушкин"
        }, Is.EqualTo(result));
    }

    [Test]
    public void ShouldParseEmailWithoutName()
    {
        var result = Parser.Parse("a.pushkin@infinnity.ru");

        Assert.That(new Email
        {
            Address = "a.pushkin@infinnity.ru"
        }, Is.EqualTo(result));
    }

    [Test]
    public void ShouldParseEmailWithIncorrectName()
    {
        var result = Parser.Parse("a.pushkin@infinnity.ru (Александр Пушкин");

        Assert.That(new Email
        {
            Address = "a.pushkin@infinnity.ru"
        }, Is.EqualTo(result));
    }

    [Test]
    public void ShouldThrowErrorForIncorrectEmail()
    {
        Assert.Throws<FormatException>(
            () => Parser.Parse("a.pushkin@companyru (Александр Пушкин"),
            "Address \"a.pushkin@companyru (Александр Пушкин\" has invalid format"
        );
    }
    

    [Test]
    public void ShouldParseManyEmails()
    {
        var result = Parser.ParseMany(
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