using Secretary.WorkingCalendar.Models;

namespace Secretary.WorkingCalendar.Tests.Models;

public class DayTests
{
    [Test]
    public void IsHolidayNotWorkingDay()
    {
        var day = new Day();

        day.Type = 1;

        var result = day.IsWorkingDay();
        
        Assert.That(result, Is.False);
    }
    
    [Test]
    public void IsSaturdayNotWorkingDay()
    {
        var day = new Day();

        day.FullDate = new DateOnly(2022, 9, 24);

        var result = day.IsWorkingDay();
        
        Assert.That(result, Is.False);
    }
    
    [Test]
    public void IsSaturdayType3WorkingDay()
    {
        var day = new Day();

        day.Type = 3;
        day.FullDate = new DateOnly(2022, 9, 24);

        var result = day.IsWorkingDay();
        
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void IsSundayNotWorkingDay()
    {
        var day = new Day();

        day.FullDate = new DateOnly(2022, 9, 25);

        var result = day.IsWorkingDay();
        
        Assert.That(result, Is.False);
    }
    
    [Test]
    public void IsSundayType3WorkingDay()
    {
        var day = new Day();

        day.Type = 3;
        day.FullDate = new DateOnly(2022, 9, 25);

        var result = day.IsWorkingDay();
        
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void IsProgrammerDay()
    {
        var day = new Day();

        day.Type = 3;
        day.FullDate = new DateOnly(2022, 9, 13);

        var result = day.IsProgrammerDay();
        
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void IsProgrammerDay2()
    {
        var day = new Day();

        day.Type = 3;
        day.FullDate = new DateOnly(2024, 9, 12);

        var result = day.IsProgrammerDay();
        
        Assert.That(result, Is.True);
    }
}