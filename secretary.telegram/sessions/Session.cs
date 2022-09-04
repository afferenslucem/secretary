using secretary.telegram.commands;

namespace secretary.telegram.sessions;

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