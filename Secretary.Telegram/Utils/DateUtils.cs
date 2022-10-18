namespace Secretary.Telegram.Utils;

public class DateUtils
{
    public static DateTime DateTimeEKB => DateTime.UtcNow.AddHours(5);

    public static DateOnly DateEKB => DateOnly.FromDateTime(DateTimeEKB);

    public static DateOnly GetStartOfWeek(DateOnly date)
    {
        var ruDayOfWeek = ((int)date.DayOfWeek + 6) % 7;
        var weekStart = date.AddDays(-ruDayOfWeek);

        return weekStart;
    }
}