namespace secretary.telegram;

public interface ITelegramClient
{
    Task SendMessage(long chatId, string message);
    Task SendDocument(long chatId, string path, string fileName);
    Task SendMessageWithKeyBoard(long chatId, string message, string[] choises);
}