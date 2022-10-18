using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram;

public delegate Task MessageReceive(UserMessage message);

public interface ITelegramClient
{
    event MessageReceive OnMessage;
    
    DateTime LastCheckTime { get; }

    Task RunDriver();
    
    Task<Message> SendMessage(
        long chatId, 
        string message
    );
    
    Task<Message> SendMessage(
        long chatId, 
        string message, 
        ReplyKeyboardMarkup replyMarkup = null
    );
    
    Task<Message> SendMessage(
        long chatId, 
        string message, 
        InlineKeyboardMarkup inlineMarkup = null
    );

    Task EditMessage(
        long chatId,
        int messageId,
        string message
    );
    
    Task EditMessage(
        long chatId, 
        int messageId,
        string message, 
        InlineKeyboardMarkup inlineMarkup
    );

    public Task EditMessageKeyboard(
        long chatId,
        int messageId,
        InlineKeyboardMarkup inlineKeyboardMarkup
    );
    
    Task SendDocument(long chatId, string path, string fileName);
    Task SendSticker(long chatId, string stickerId);
    
    Task DeleteMessage(long chatId, int messageId);
}