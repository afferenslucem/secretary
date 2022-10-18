namespace Secretary.Telegram;

public class UserMessage
{
    public long ChatId;
    public string From = null!;
    public string Text = null!;
    
    public bool IsCallback { get; private set; }
    public int? CallbackMessageId;
    public int? MessageId;

    public bool IsCommand { get; private set; }
    public bool HasArguments { get; private set; }
    public string? CommandText { get; private set; }
    public string? CommandArgument { get; private set; }
    
    public UserMessage(long chatId, string from, string text, int? callbackMessageId = null, int? messageId = null)
    {
        ChatId = chatId;
        MessageId = messageId;
        From = from;
        Text = text;
        
        CallbackMessageId = callbackMessageId;
        IsCallback = callbackMessageId.HasValue;

        IsCommand = Text.StartsWith("/");

        if (IsCommand)
        {
            ReadCommand();
        }
    }
    
    public UserMessage() { }

    private void ReadCommand()
    {
        var spaceIndex = Text.IndexOf(' ');

        if (spaceIndex == -1)
        {
            CommandText = Text;
            HasArguments = false;
        }
        else
        {
            CommandText = Text.Substring(0, spaceIndex);
            CommandArgument = Text.Substring(spaceIndex + 1);
            HasArguments = true;
        }
    }
}