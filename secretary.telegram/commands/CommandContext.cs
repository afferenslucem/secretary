using secretary.cache;
using secretary.storage;
using secretary.telegram.sessions;
using secretary.yandex.authentication;
using secretary.yandex.mail;

namespace secretary.telegram.commands;

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