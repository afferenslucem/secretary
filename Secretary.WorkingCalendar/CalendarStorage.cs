using Secretary.Logging;
using Secretary.WorkingCalendar.Models;
using Serilog;

namespace Secretary.WorkingCalendar;

public class CalendarStorage
{
    private static ILogger _logger = LogPoint.GetLogger<CalendarStorage>();

    private static Dictionary<int, Calendar> _calendars = new ();

    public static Calendar GetCalendar(DateOnly date)
    {
        var result = GetFromCache(date.Year);

        return result ?? ReadCalendar(date.Year);
    }

    private static Calendar? GetFromCache(int year)
    {
        _logger.Debug($"Try find calendar {year} in cache");
        
        Calendar? result;

        if (_calendars.TryGetValue(year, out result))
        {
            _logger.Debug($"Calendar {year} found in cache");
            return result;
        }
        else
        {
            _logger.Debug($"Could not find calendar {year}");
            return null;
        }
    }

    private static Calendar ReadCalendar(int year)
    {
        _logger.Debug($"Reading calendar {year}");
        
        var reader = new CalendarReader();

        var calendar = reader.Read(year);

        _calendars[year] = calendar;
        
        _logger.Debug($"Calendar {year} saved to cache");

        return calendar;
    }
}