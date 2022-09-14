namespace Secretary.Telegram.Exceptions;

public class DatePeriodParseException: Exception
{
    public string ParsedText { get; init; }

    public DatePeriodParseException(string parsedText)
    {
        ParsedText = parsedText;
    }
}