using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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

    public Task<Message> SendMessage(
        string message
    )
    {
        return _telegramClient.SendMessage(_chatId, message);
    }

    public Task<Message> SendMessage(
        string message, 
        ReplyKeyboardMarkup replyMarkup
    )
    {
        return _telegramClient.SendMessage(_chatId, message, replyMarkup);
    }

    public Task<Message> SendMessage(
        string message, 
        InlineKeyboardMarkup inlineKeyboardMarkup
    )
    {
        return _telegramClient.SendMessage(_chatId, message, inlineKeyboardMarkup);
    }

    public Task SendDocument(string path, string fileName)
    {
        return _telegramClient.SendDocument(_chatId, path, fileName);
    }

    public Task SendSticker(string stickerId)
    {
        return _telegramClient.SendSticker(_chatId, stickerId);
    }
    public Task DeleteMessage(int messageId)
    {
        return _telegramClient.DeleteMessage(_chatId, messageId);
    }
    public Task EditMessage(int messageId, string text)
    {
        return _telegramClient.EditMessage(_chatId, messageId, text);
    }
    public Task EditMessage(int messageId, string text, InlineKeyboardMarkup markup)
    {
        return _telegramClient.EditMessage(_chatId, messageId, text, markup);
    }
}