namespace secretary.telegram.exceptions;

public class DatePeriodParseException: Exception
{
    public string ParsedText { get; init; }

    public DatePeriodParseException(string parsedText)
    {
        ParsedText = parsedText;
    }
}