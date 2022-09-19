using Secretary.Cache;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Telegram;
using Secretary.Telegram.Sessions;
using Secretary.Telegram.Wrappers;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Mail;

namespace Secretary.Telegram.Commands;

public class CommandContext
{
    public long ChatId;

    public ITelegramClient TelegramClient = null!;

    public ISessionStorage SessionStorage = null!;

    public IYandexAuthenticator YandexAuthenticator = null!;

    public IUserStorage UserStorage = null!;
    
    public IDocumentStorage DocumentStorage = null!;
    
    public IEmailStorage EmailStorage = null!;

    public IMailClient MailClient = null!;

    public IEventLogStorage EventLogStorage = null!;
    
    public ICacheService CacheService = null!;

    public string Message = null!;

    public CommandContext(
        long chatId, 
        ITelegramClient telegramClient, 
        ISessionStorage sessionStorage, 
        IUserStorage userStorage, 
        IDocumentStorage documentStorage, 
        IEmailStorage emailStorage,
        IEventLogStorage eventLogStorage,
        IYandexAuthenticator yandexAuthenticator, 
        IMailClient mailClient, 
        ICacheService cacheService,
        string message)
    {
        ChatId = chatId;
        TelegramClient = telegramClient;
        SessionStorage = sessionStorage;
        UserStorage = userStorage;
        DocumentStorage = documentStorage;
        EmailStorage = emailStorage;
        YandexAuthenticator = yandexAuthenticator;
        MailClient = mailClient;
        EventLogStorage = eventLogStorage;
        CacheService = cacheService;
        Message = message;
    }
    
    /**
     * Сделан для тестов
     * @deprecated
     */
    public CommandContext() { }
}