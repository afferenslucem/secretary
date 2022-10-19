namespace Secretary.Utils;

public class DateUtils
{
    public static DateTime DateTimeEkb => TimeZoneInfo.ConvertTime(DateTime.Now, EkbTimeZone);

    public static DateOnly DateEkb => DateOnly.FromDateTime(DateTimeEkb);

    public static TimeZoneInfo EkbTimeZone =
        TimeZoneInfo.CreateCustomTimeZone("Ekaterinburg", TimeSpan.FromHours(5), null, null);

    public static DateTime ConvertToEkbTime(DateTime dateTime)
    {
        return TimeZoneInfo.ConvertTime(dateTime, EkbTimeZone);
    }
    
    public static DateTime? ConvertToEkbTime(DateTime? dateTime)
    {
        return dateTime.HasValue ? ConvertToEkbTime(dateTime.Value) : null;
    }

    public static DateOnly GetStartOfWeek(DateOnly date)
    {
        var ruDayOfWeek = ((int)date.DayOfWeek + 6) % 7;
        var weekStart = date.AddDays(-ruDayOfWeek);

        return weekStart;
    }
}