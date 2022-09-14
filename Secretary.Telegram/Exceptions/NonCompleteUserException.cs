namespace Secretary.Telegram.Exceptions;

public class NonCompleteUserException: Exception
{
    public NonCompleteUserException(string message): base(message) {}
}