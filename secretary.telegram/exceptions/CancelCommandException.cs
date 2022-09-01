namespace secretary.telegram.exceptions;

public class CancelCommandException: Exception
{
    private string _commandName;

    public string CommandName => _commandName;

    public CancelCommandException(string commandName)
    {
        _commandName = commandName;
    }
}