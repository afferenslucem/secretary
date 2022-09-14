using Secretary.Telegram.Commands;

namespace Secretary.Telegram.Sessions;

public class Session
{
    public long ChatId { get; set; }
    public Command LastCommand { get; set; }
    
    public Session(long chatId, Command lastCommand)
    {
        ChatId = chatId;
        LastCommand = lastCommand;
    }
}