namespace Secretary.Telegram.Wrappers;

public class TelegramClientWrapper
{
    private ITelegramClient _telegramClient;
    private long _chatId;

    public TelegramClientWrapper(ITelegramClient telegramClient, long chatId)
    {
        _chatId = chatId;
        _telegramClient = telegramClient;
    }

    public Task SendMessage(string message)
    {
        return _telegramClient.SendMessage(_chatId, message);
    }

    public Task SendMessageWithKeyBoard(string message, string[] choices)
    {
        return _telegramClient.SendMessageWithKeyBoard(_chatId, message, choices);
    }

    public Task SendDocument(string path, string fileName)
    {
        return _telegramClient.SendDocument(_chatId, path, fileName);
    }

    public Task SendSticker(string stickerId)
    {
        return _telegramClient.SendSticker(_chatId, stickerId);
    }
}