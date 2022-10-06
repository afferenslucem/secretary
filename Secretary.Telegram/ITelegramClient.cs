using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram;

public delegate Task MessageReceive(BotMessage message);

public interface ITelegramClient
{
    event MessageReceive OnMessage;
    
    DateTime LastCheckTime { get; }

    Task RunDriver();
    
    Task SendMessage(
        long chatId, 
        string message
    );
    
    Task SendMessage(
        long chatId, 
        string message, 
        ReplyKeyboardMarkup replyMarkup = null
    );
    
    Task SendMessage(
        long chatId, 
        string message, 
        InlineKeyboardMarkup inlineMarkup = null
    );
    
    Task SendDocument(long chatId, string path, string fileName);
    Task SendSticker(long chatId, string stickerId);
}