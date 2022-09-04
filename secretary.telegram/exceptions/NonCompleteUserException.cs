namespace secretary.telegram.exceptions;

public class NonCompleteUserException: Exception
{
    public NonCompleteUserException(string message): base(message) {}
}