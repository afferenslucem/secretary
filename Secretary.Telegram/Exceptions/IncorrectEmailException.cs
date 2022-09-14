namespace Secretary.Telegram.Exceptions;

public class IncorrectEmailException: Exception
{
    private string _incorrectEmail;

    public string IncorrectEmail => _incorrectEmail;

    public IncorrectEmailException(string incorrectEmail, string message): base(message)
    {
        _incorrectEmail = incorrectEmail;
    }
}