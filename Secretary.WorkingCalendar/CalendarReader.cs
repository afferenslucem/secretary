using System.Xml.Serialization;
using Secretary.Configuration;
using Secretary.Logging;
using Secretary.WorkingCalendar.Models;
using Serilog;

namespace Secretary.WorkingCalendar;

public class CalendarReader: ICalendarReader
{
    private ILogger _logger = LogPoint.GetLogger<CalendarReader>();
    
    public Calendar Read(int year)
    {
        _logger.Information($"Read calendar {year}");
        
        XmlSerializer serializer =
            new XmlSerializer(typeof(Calendar));

        var path = $@"{Config.Instance.CalendarsPath}/{year}.xml";
        
        _logger.Debug($"Reading file {path}");
        using Stream reader = new FileStream(path, FileMode.Open);
        
        _logger.Debug($"Deserializing {path}");
        var result = (Calendar)serializer.Deserialize(reader);

        result!.Initialize();
        
        return result;
    }
}