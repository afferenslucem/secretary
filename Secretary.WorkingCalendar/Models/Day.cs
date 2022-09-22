using System.Xml.Serialization;

namespace Secretary.WorkingCalendar.Models;

[XmlType(TypeName = "day")]
public class Day
{
    [XmlAttribute("d")] public string Date { get; set; } = null!;

    [XmlAttribute("t")] public int Type { get; set; }

    [XmlAttribute("h")] public int HolidayId { get; set; }

    [XmlAttribute("f")] public string? From { get; set; }
    
    public DateOnly FullDate { get; set; }

    internal void InitializeForYear(int year)
    {
        var dateString = $"{year}.{Date}";
        
        this.FullDate = DateOnly.ParseExact(dateString, "yyyy.MM.dd");
    }

    public override string ToString()
    {
        return FullDate.ToString();
    }

    public bool IsWorkingDay()
    {
        if (Type == 1)
        {
            return false;
        }

        if (Type == 3)
        {
            return true;
        }

        if (FullDate.DayOfWeek == DayOfWeek.Saturday)
        {
            return false;
        }

        if (FullDate.DayOfWeek == DayOfWeek.Sunday)
        {
            return false;
        }
        
        return true;
    }

    public bool IsProgrammerDay()
    {
        return FullDate.DayOfYear == 256;
    }
}