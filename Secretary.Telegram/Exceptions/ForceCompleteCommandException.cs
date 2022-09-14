namespace Secretary.Telegram.Exceptions;

public class ForceCompleteCommandException: Exception
{
    private string _commandName;

    public string CommandName => _commandName;

    public ForceCompleteCommandException(string commandName)
    {
        _commandName = commandName;
    }
}