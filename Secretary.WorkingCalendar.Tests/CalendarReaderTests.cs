namespace Secretary.WorkingCalendar.Tests;

public class CalendarReaderTests
{
    private CalendarReader _reader = null;
    
    [SetUp]
    public void Setup()
    {
        _reader = new CalendarReader();
    }

    [Test]
    public void ShouldReadAllData()
    {
        var calendar = _reader.Read(2022);
        
        Assert.That(calendar.Year, Is.EqualTo(2022));
        Assert.That(calendar.Holidays.Length, Is.EqualTo(8));
        Assert.That(calendar.Days.Length, Is.EqualTo(22));
    }

    [Test]
    public void ShouldReadHoliday()
    {
        var calendar = _reader.Read(2022);

        var holiday = calendar.Holidays[0];
        
        Assert.That(holiday.Id, Is.EqualTo(1));
        Assert.That(holiday.Title, Is.EqualTo("Новогодние каникулы"));
    }

    [Test]
    public void ShouldReadDay()
    {
        var calendar = _reader.Read(2022);

        var holiday = calendar.Days[10];
        
        Assert.That(holiday.Date, Is.EqualTo("03.05"));
        Assert.That(holiday.FullDate, Is.EqualTo(new DateOnly(2022, 3, 5)));
        Assert.That(holiday.Type, Is.EqualTo(2));
    }

    [Test]
    public void ShouldReadDayWithHolidayId()
    {
        var calendar = _reader.Read(2022);

        var holiday = calendar.Days[0];
        
        Assert.That(holiday.Date, Is.EqualTo("01.01"));
        Assert.That(holiday.Type, Is.EqualTo(1));
        Assert.That(holiday.HolidayId, Is.EqualTo(1));
    }

    [Test]
    public void ShouldReadDayWithFrom()
    {
        var calendar = _reader.Read(2022);

        var holiday = calendar.Days[11];
        
        Assert.That(holiday.Date, Is.EqualTo("03.07"));
        Assert.That(holiday.Type, Is.EqualTo(1));
        Assert.That(holiday.From, Is.EqualTo("03.05"));
    }
}