namespace Secretary.Telegram;

public class BotMessage
{
    public readonly long ChatId;
    public readonly string From;
    public readonly string Text;
    
    
    public BotMessage(long chatId, string from, string text)
    {
        ChatId = chatId;
        From = from;
        Text = text;
    }
}