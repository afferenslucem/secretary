using System.Globalization;
using System.Text.RegularExpressions;
using secretary.telegram.models;

namespace secretary.telegram.utils;

public class DatePeriodParser
{
    private readonly Regex _datePattern = new(@"\d{1,2}\.\d{1,2}\.\d{2,4}");
    private readonly Regex _timePattern = new(@"\d{1,2}:\d{2}");
    
    public DatePeriod Parse(string period)
    {
        var dateMatches = _datePattern.Matches(period);

        if (dateMatches.Count == 1)
        {
            return ParseSingleDate(dateMatches[0], period);
        }
        else if (dateMatches.Count == 2)
        {
            return ParsePeriod(dateMatches, period);
        }

        return null!;
    }

    private DatePeriod ParseSingleDate(Match dateMatch, string raw)
    {
        var times = _timePattern.Matches(raw);

        if (times.Count == 2)
        {
            var startDate = ParseDate($"{dateMatch.Value} {times[0].Value}");
            var endDate = ParseDate($"{dateMatch.Value} {times[1].Value}");

            return new DatePeriod(startDate, endDate, raw);
        }
        else
        {
            var date = ParseDate(dateMatch.Value);

            return new DatePeriod(date, date, raw);
        }
    }

    private DatePeriod ParsePeriod(MatchCollection periods, string raw)
    {
        var times = _timePattern.Matches(raw);

        if (times.Count == 2)
        {
            var startDate = ParseDate($"{periods[0].Value} {times[0].Value}");
            var endDate = ParseDate($"{periods[1].Value} {times[1].Value}");

            return new DatePeriod(startDate, endDate, raw);
        }
        else
        {
            var startDate = ParseDate(periods[0].Value);
            var endDate = ParseDate(periods[1].Value);

            return new DatePeriod(startDate, endDate, raw);
        }
    }

    private DateTime ParseDate(string date)
    {
        return DateTime.Parse(date, new CultureInfo("ru-RU"));
    }
}