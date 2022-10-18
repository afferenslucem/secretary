using Secretary.WorkingCalendar.Models;

namespace Secretary.WorkingCalendar.Tests.Models;

public class CalendarTests
{
    [Test]
    public void FindOrCreateShouldReturnCalendarDay()
    {
        var calendar = new Calendar();

        calendar.Days = new[]
        {
            new Day() { FullDate = new DateOnly(2022, 10, 14) }
        };

        var result = calendar.FindOrCreate(new DateOnly(2022, 10, 14));
        
        Assert.That(result.FullDate, Is.EqualTo(new DateOnly(2022, 10, 14)));
    }
    
    [Test]
    public void FindOrCreateShouldCreateCalendarDay()
    {
        var calendar = new Calendar();

        calendar.Days = new[]
        {
            new Day() { FullDate = new DateOnly(2022, 10, 14) }
        };

        var result = calendar.FindOrCreate(new DateOnly(2022, 2, 14));
        
        Assert.That(result.FullDate, Is.EqualTo(new DateOnly(2022, 2, 14)));
    }
    
    [Test]
    public void FridayOfOctober2022IsLastWorkingDay()
    {
        var calendar = new Calendar()
        {
            Days = new Day[] { }
        };
    
        var result = calendar.IsLastWorkingDayBefore(
            new DateOnly(2022, 10, 14),
            new DateOnly(2022, 10, 15)
        );
        
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void ThursdayOfOctober2022IsLastWorkingDay()
    {
        var calendar = new Calendar()
        {
            Days = new Day[] { }
        };
    
        var result = calendar.IsLastWorkingDayBefore(
            new DateOnly(2022, 10, 13),
            new DateOnly(2022, 10, 15)
        );
        
        Assert.That(result, Is.False);
    }
    
    [Test]
    public void FridayOfSeptember2023IsLastWorkingDay()
    {
        var calendar = new Calendar()
        {
            Days = new Day[] { }
        };
    
        var result = calendar.IsLastWorkingDayBefore(
            new DateOnly(2023, 9, 15),
            new DateOnly(2023, 9, 15)
        );
        
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void ThursdayOfSeptember2023IsLastWorkingDayWithHolidayFriday()
    {
        var calendar = new Calendar()
        {
            Days = new []
            {
                new Day() { FullDate = new DateOnly(2023, 9, 15), Type = 1}
            }
        };
    
        var result = calendar.IsLastWorkingDayBefore(
            new DateOnly(2023, 9, 14),
            new DateOnly(2023, 9, 15)
        );
        
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void ShouldReturn11()
    {
        var calendar = CalendarStorage.GetCalendar(new DateOnly(2022, 10, 18));

        var result = calendar.GetWorkingDays(new DateOnly(2022, 10, 16), new DateOnly(2022, 10, 31));
        
        Assert.That(result, Is.EqualTo(11));
    }
    
    [Test]
    public void ShouldReturn5()
    {
        var calendar = CalendarStorage.GetCalendar(new DateOnly(2023, 1, 13));

        var result = calendar.GetWorkingDays(new DateOnly(2023, 1, 1), new DateOnly(2023, 1, 13));
        
        Assert.That(result, Is.EqualTo(5));
    }
}