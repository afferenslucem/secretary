using System.Xml.Serialization;

namespace Secretary.WorkingCalendar.Models;

[XmlType(TypeName = "calendar")]
public class Calendar
{
    [XmlArray("holidays")] public Holiday[] Holidays { get; set; } = null!;

    [XmlArray("days")] public Day[] Days { get; set; } = null!;

    [XmlAttribute("year")] public int Year { get; set; }

    internal void Initialize()
    {
        foreach (var day in Days)
        {
            day.InitializeForYear(Year);
        }
    }

    public override string ToString()
    {
        return Year.ToString();
    }

    public bool IsLastWorkingDayBefore(DateOnly now, DateOnly lastDate)
    {
        var nowDT = now.ToDateTime(TimeOnly.MinValue);
        var lastDT = lastDate.ToDateTime(TimeOnly.MinValue);
        
        var days = Enumerable.Range(0, lastDT.Subtract(nowDT).Days + 1)
            .Select(offset => nowDT.AddDays(offset))
            .Select(dt => DateOnly.FromDateTime(dt))
            .Select(date => FindOrCreate(date))
            .ToArray();

        var lastDay = days.LastOrDefault(day => day.IsWorkingDay());

        if (lastDay == null)
        {
            return false;
        }
        
        return lastDay.FullDate == now;
    }

    public Day FindOrCreate(DateOnly date)
    {
        var day = Days.FirstOrDefault(item => item.FullDate == date);

        return day ?? new Day() { FullDate = date };
    }
}