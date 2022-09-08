using secretary.telegram.models;

namespace secretary.telegram.tests.commands.models;

public class DatePeriodTests
{
    [Test]
    public void ShouldReturnIsOneDayTrue()
    {
        var start = new DateTime(2022, 9, 8, 9, 0, 0);
        var end = new DateTime(2022, 9, 8, 13, 0, 0);
        
        var period = new DatePeriod(start, end, "");
        
        Assert.That(period.IsOneDay, Is.True);
    }
    
    [Test]
    public void ShouldReturnIsOneDayFalse()
    {
        var start = new DateTime(2022, 9, 8, 9, 0, 0);
        var end = new DateTime(2022, 9, 13, 13, 0, 0);
        
        var period = new DatePeriod(start, end, "");
        
        Assert.That(period.IsOneDay, Is.False);
    }
    
    [Test]
    public void ShouldReturnOneDayPeriodString()
    {
        var start = new DateTime(2022, 9, 8, 9, 0, 0);
        var end = new DateTime(2022, 9, 8, 13, 0, 0);
        
        var period = new DatePeriod(start, end, "");
        
        Assert.That(period.DayPeriod, Is.EqualTo("08.09.2022"));
    }
    
    [Test]
    public void ShouldReturnPeriodString()
    {
        var start = new DateTime(2022, 9, 8, 9, 0, 0);
        var end = new DateTime(2022, 9, 13, 13, 0, 0);
        
        var period = new DatePeriod(start, end, "");
        
        Assert.That(period.DayPeriod, Is.EqualTo("08.09.2022 — 13.09.2022"));
    }
}