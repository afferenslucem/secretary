namespace Secretary.Telegram.Commands.Abstractions;

public class ExecuteDirection
{
    public const int RunNext = 1;
    public const int Retry = 0;
    public const int GoBack = -1;
}