using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Models;
using Secretary.Telegram.Utils;

namespace Secretary.Telegram.Tests.Utils;

public class DatePeriodParserTests
{
    private DatePeriodParser _parser = null!;

    [SetUp]
    public void Setup()
    {
        _parser = new DatePeriodParser();
    }

    [Test]
    public void ShouldParseSingleDate()
    {
        var line = "08.09.2022";

        var expected = new DatePeriod(
            new DateTime(2022, 09, 08),
            new DateTime(2022, 09, 08),
            line
        );

        var result = _parser.Parse(line);
        
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ShouldParseSingleDateWithTimes()
    {
        var line = "08.09.2022 с 9:00 до 13:00";

        var expected = new DatePeriod(
            new DateTime(2022, 09, 08, 9, 00, 00),
            new DateTime(2022, 09, 08, 13, 00, 00),
            line
        );

        var result = _parser.Parse(line);
        
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ShouldParseSinglePeriod()
    {
        var line = "с 08.09.2022 до 10.09.2022";

        var expected = new DatePeriod(
            new DateTime(2022, 09, 08),
            new DateTime(2022, 09, 10),
            line
        );

        var result = _parser.Parse(line);
        
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ShouldParsePeriodWithTimes()
    {
        var line = "с 09:00 08.09.2022 до 13:00 10.09.2022";

        var expected = new DatePeriod(
            new DateTime(2022, 09, 08, 9, 0, 0),
            new DateTime(2022, 09, 10, 13, 0, 0),
            line
        );

        var result = _parser.Parse(line);
        
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ShouldThrowDatePeriodParseException()
    {
        var line = "hello";

        Assert.Throws<DatePeriodParseException>(() => _parser.Parse(line));
    }

    [Test]
    public void ShouldThrowFormatException()
    {
        var line = "20.22.2022";

        Assert.Throws<FormatException>(() => _parser.Parse(line));
    }
}