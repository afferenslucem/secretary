using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Abstractions;

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