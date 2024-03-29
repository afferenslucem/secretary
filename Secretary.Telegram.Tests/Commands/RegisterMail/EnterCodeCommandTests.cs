﻿using Moq;
using Secretary.Cache;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.RegisterMail;
using Secretary.Telegram.Sessions;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Exceptions;

namespace Secretary.Telegram.Tests.Commands.RegisterMail;

public class EnterCodeCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<IYandexAuthenticator> _yandexAuthenticator = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<ICacheService> _cacheService = null!;
    private CommandContext _context = null!;
    private EnterCodeCommand _command = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();
        _sessionStorage = new Mock<ISessionStorage>();
        _yandexAuthenticator = new Mock<IYandexAuthenticator>();
        _cacheService = new Mock<ICacheService>();

        _context = new CommandContext()
        {
            UserMessage = new UserMessage { ChatId = 2517},
            TelegramClient = _client.Object, 
            YandexAuthenticator = _yandexAuthenticator.Object, 
            UserStorage = _userStorage.Object,
            SessionStorage = _sessionStorage.Object,
            CacheService = _cacheService.Object,
        };

        _command = new EnterCodeCommand();
        
        _command.Context = _context;
    }
    
    [Test]
    public async Task ShouldSendCode()
    {
        _yandexAuthenticator.Setup(target => target.GetAuthenticationCode(It.IsAny<CancellationToken>())).ReturnsAsync(
            new AuthenticationData()
            {
                user_code = "code",
                verification_url = "url",
            }
        );

        _yandexAuthenticator.Setup(target => target.CheckToken(It.IsAny<AuthenticationData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenData() { access_token = "token"});
        
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Пожалуйста, <strong>УБЕДИТЕСЬ</strong>, что вы авторизуетесь в рабочей почте!\n" +
                                                              "Введите этот код: <code>code</code> в поле ввода по этой ссылке: url. Регистрация может занять пару минут."));
    }
        
    [Test]
    public async Task ShouldSendSorryForYandexGetAuthCode()
    {
        _yandexAuthenticator
            .Setup(target => target.GetAuthenticationCode(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new YandexAuthenticationException());
        
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, 
            "При запросе токена для авторизации произошла ошибка:(\n" +
            "Попробуйте через пару минут, если не сработает, то обратитесь по вот этому адресу @hrodveetnir"));
    }
    
        
    [Test]
    public async Task ShouldSendDone()
    {
        _yandexAuthenticator.Setup(target => target.GetAuthenticationCode(It.IsAny<CancellationToken>())).ReturnsAsync(
            new AuthenticationData()
            {
                user_code = "code",
                verification_url = "url",
                expires_in = 300,
            }
        );

        var oldUser = new User
        {
            ChatId = 2517,
            Name = "Александр Пушкин"
        };

        _userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);

        _cacheService.Setup(target => target.GetEntity<RegisterMailCache>(It.IsAny<long>()))
            .ReturnsAsync(new RegisterMailCache("a.pushkin@infinnity.ru"));
        
        _yandexAuthenticator.Setup(target => target.CheckToken(It.IsAny<AuthenticationData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenData() { access_token = "token"});
        
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Ура, токен для почты получен!"));
    }
    
    [Test]
    public async Task ShouldUpdateUser()
    {
        _yandexAuthenticator.Setup(target => target.CheckToken(It.IsAny<AuthenticationData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenData() { access_token = "access_token", refresh_token = "refresh_token", expires_in = 500 });
        
        _yandexAuthenticator.Setup(target => target.GetAuthenticationCode(It.IsAny<CancellationToken>())).ReturnsAsync(
            new AuthenticationData()
            {
                user_code = "code",
                verification_url = "url",
                expires_in = 300,
            }
        );
        
        var oldUser = new User
        {
            ChatId = 2517,
            Name = "Александр Пушкин"
        };
        _userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);
        
        _cacheService.Setup(target => target.GetEntity<RegisterMailCache>(It.IsAny<long>()))
            .ReturnsAsync(new RegisterMailCache("a.pushkin@infinnity.ru"));
        
        await _command.Execute();
        
        _userStorage.Verify(target => target.SetUser(It.Is<User>(
            user => user.ChatId == 2517 
                    && user.Name == "Александр Пушкин" 
                    && user.AccessToken == "access_token" 
                    && user.RefreshToken == "refresh_token"
                    && user.Email == "a.pushkin@infinnity.ru"
        )));
        
        _cacheService.Verify(target => target.DeleteEntity<RegisterMailCache>(2517), Times.Once);
    }
    
        
    [Test]
    public async Task ShouldInsertUser()
    {
        _yandexAuthenticator.Setup(target => target.CheckToken(It.IsAny<AuthenticationData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenData() { access_token = "access_token", refresh_token = "refresh_token", expires_in = 500 });
        
        _yandexAuthenticator.Setup(target => target.GetAuthenticationCode(It.IsAny<CancellationToken>())).ReturnsAsync(
            new AuthenticationData()
            {
                user_code = "code",
                verification_url = "url",
                expires_in = 300,
            }
        );
        
        User? oldUser = null;
        _userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);
        
        _cacheService.Setup(target => target.GetEntity<RegisterMailCache>(It.IsAny<long>()))
            .ReturnsAsync(new RegisterMailCache("a.pushkin@infinnity.ru"));
        
        await _command.Execute();
        
        _userStorage.Verify(target => target.SetUser(It.Is<User>(
            user => user.ChatId == 2517 
                    && user.AccessToken == "access_token" 
                    && user.RefreshToken == "refresh_token"
                    && user.Email == "a.pushkin@infinnity.ru"
        )));
    }
    
    [Test]
    public async Task ShouldSendSorryForYandexCheckCode()
    {
        _yandexAuthenticator.Setup(target => target.GetAuthenticationCode(It.IsAny<CancellationToken>())).ReturnsAsync(
            new AuthenticationData()
            {
                user_code = "code",
                verification_url = "url",
                expires_in = 300,
            }
        );
        
        _yandexAuthenticator
            .Setup(target =>  target.CheckToken(It.IsAny<AuthenticationData>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new YandexAuthenticationException());
        
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, 
            "При запросе токена для авторизации произошла ошибка:(\n" +
            "Попробуйте через пару минут, если не сработает, то обратитесь по вот этому адресу @hrodveetnir"));
    }
}