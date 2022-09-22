using Secretary.WorkingCalendar.Models;

namespace Secretary.WorkingCalendar;

public interface ICalendarReader
{
    Calendar Read(int year);
}