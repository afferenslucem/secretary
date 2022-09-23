using System.Xml.Serialization;
using Secretary.Configuration;
using Secretary.WorkingCalendar.Models;

namespace Secretary.WorkingCalendar;

public class CalendarReader: ICalendarReader
{
    public Calendar Read(int year)
    {
        XmlSerializer serializer =
            new XmlSerializer(typeof(Calendar));

        var path = $@"{Config.Instance.CalendarsPath}/{year}.xml";

        using Stream reader = new FileStream(path, FileMode.Open);
        
        var result = (Calendar)serializer.Deserialize(reader);

        result!.Initialize();
        
        return result;
    }
}