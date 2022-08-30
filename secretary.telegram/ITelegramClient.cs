namespace secretary.telegram;

public delegate void MessageReceive(BotMessage message);

public interface ITelegramClient
{
    event MessageReceive OnMessage;

    Task RunDriver();
    
    Task SendMessage(long chatId, string message);
    Task SendDocument(long chatId, string path, string fileName);
    Task SendMessageWithKeyBoard(long chatId, string message, string[] choises);
}