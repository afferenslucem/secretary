using Secretary.Cache;
using Secretary.Storage;
using Secretary.Telegram;
using Secretary.Telegram.Sessions;
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

    public ICacheService CacheService = null!;

    public string Message = null!;

    public bool BackwardRedirect = false;

    public CommandContext(
        long chatId, 
        ITelegramClient telegramClient, 
        ISessionStorage sessionStorage, 
        IUserStorage userStorage, 
        IDocumentStorage documentStorage, 
        IEmailStorage emailStorage,
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
        CacheService = cacheService;
        Message = message;
    }
    
    /**
     * Сделан для тестов
     * @deprecated
     */
    public CommandContext() { }
}