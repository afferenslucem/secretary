﻿using Secretary.Cache;
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
    public long ChatId => UserMessage.ChatId;
    public string TelegramUserName => UserMessage.From;
    public string Message => UserMessage.Text;
    public int? CallbackMessageId => UserMessage.CallbackMessageId;

    public UserMessage UserMessage = null!;

    public ITelegramClient TelegramClient = null!;

    public ISessionStorage SessionStorage = null!;

    public IYandexAuthenticator YandexAuthenticator = null!;

    public IUserStorage UserStorage = null!;
    
    public IDocumentStorage DocumentStorage = null!;
    
    public IEmailStorage EmailStorage = null!;

    public IMailSender MailSender = null!;

    public IEventLogStorage EventLogStorage = null!;
    
    public ICacheService CacheService = null!;

    public CommandContext(
        UserMessage userMessage,
        ITelegramClient telegramClient, 
        ISessionStorage sessionStorage, 
        IUserStorage userStorage, 
        IDocumentStorage documentStorage, 
        IEmailStorage emailStorage,
        IEventLogStorage eventLogStorage,
        IYandexAuthenticator yandexAuthenticator, 
        IMailSender mailSender, 
        ICacheService cacheService)
    {
        UserMessage = userMessage;
        TelegramClient = telegramClient;
        SessionStorage = sessionStorage;
        UserStorage = userStorage;
        DocumentStorage = documentStorage;
        EmailStorage = emailStorage;
        YandexAuthenticator = yandexAuthenticator;
        MailSender = mailSender;
        EventLogStorage = eventLogStorage;
        CacheService = cacheService;
    }
    
    /**
     * Сделан для тестов
     * @deprecated
     */
    public CommandContext() { }
}