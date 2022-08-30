namespace secretary.telegram;

public class BotMessage
{
    public readonly long ChatId;
    public readonly string Text;
    
    
    public BotMessage(long chatId, string text)
    {
        ChatId = chatId;
        Text = text;
    }
}