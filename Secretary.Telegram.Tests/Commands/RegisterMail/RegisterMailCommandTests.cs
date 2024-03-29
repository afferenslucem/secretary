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

namespace Secretary.Telegram.Tests.Commands.RegisterMail;

public class RegisterMailCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<IYandexAuthenticator> _yandexAuthenticator = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<ICacheService> _cacheService = null!;
    private CommandContext _context= null!;
    private RegisterMailCommand _command= null!;
        
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

        _command = new RegisterMailCommand();
        
        _command.Context = _context;
    }

    [Test]
    public void ShouldCreate()
    {
        Assert.Pass();
    }
    
    [Test]
    public void ShouldHaveCorrectKey()
    {
        Assert.That(RegisterMailCommand.Key, Is.EqualTo("/registermail"));
    }
    
    [Test]
    public async Task ShouldRunFully()
    {
        _yandexAuthenticator.Setup(target => target.IsUserDomainAllowed(It.IsAny<string>())).Returns(true);
        _userStorage.Setup(target => target.GetUser(2517)).ReturnsAsync(() => new User() {Name = "Александр Пушкин"} );
        
        _context.UserMessage.Text ="/registermail";
        await _command.Execute();
        
        _yandexAuthenticator
            .Setup(target => target.GetAuthenticationCode(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthenticationData() { expires_in = 300 } );
        _yandexAuthenticator
            .Setup(target => target.CheckToken(It.IsAny<AuthenticationData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenData() { access_token = "access", refresh_token = "refresh"});
        
        _context.UserMessage.Text ="a.pushkin@infinnity.ru";
        
        _cacheService.Setup(target => target.GetEntity<RegisterMailCache>(It.IsAny<long>()))
            .ReturnsAsync(new RegisterMailCache("a.pushkin@infinnity.ru"));
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
        
        await _command.OnMessage();
        await _command.OnComplete();
        
        _userStorage.Verify(target => target.SetUser(It.Is<User>(user => user.Name == "Александр Пушкин" 
                                                                         && user.Email == "a.pushkin@infinnity.ru" 
                                                                         && user.AccessToken == "access" 
                                                                         && user.RefreshToken == "refresh")), Times.Once);

        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
    }
    
    [Test]
    public async Task ShouldAskEmailAgainForIncorrectEmail()
    {
        _userStorage.Setup(target => target.GetUser(2517)).ReturnsAsync(() => new User());
        
        _context.UserMessage.Text ="/registermail";
        await _command.Execute();

        _context.UserMessage.Text ="a.pushkin";

        await _command.OnMessage();
        _client.Verify(target => target.SendMessage(2517, "Некорректный формат почты. Введите почту еще раз"));
    }
    
    [Test]
    public async Task ShouldPassExecutionAfterIncorrectMail()
    {
        _yandexAuthenticator.Setup(target => target.IsUserDomainAllowed(It.IsAny<string>())).Returns(true);
        _userStorage.Setup(target => target.GetUser(2517)).ReturnsAsync(() => new User());
        
        _context.UserMessage.Text ="/registermail";
        await _command.Execute();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);

        _context.UserMessage.Text ="a.pushkin";

        await _command.OnMessage();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
        _client.Verify(target => target.SendMessage(2517, "Некорректный формат почты. Введите почту еще раз"));
        
        _yandexAuthenticator
            .Setup(target => target.GetAuthenticationCode(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthenticationData() { expires_in = 300 } );
        _yandexAuthenticator
            .Setup(target => target.CheckToken(It.IsAny<AuthenticationData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenData() { access_token = "access", refresh_token = "refresh"});
        
        _context.UserMessage.Text ="a.pushkin@infinnity.ru";
        
        _cacheService.Setup(target => target.GetEntity<RegisterMailCache>(It.IsAny<long>()))
            .ReturnsAsync(new RegisterMailCache("a.pushkin@infinnity.ru"));
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
        await _command.OnMessage();
        await _command.OnComplete();
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
        _userStorage.Verify(target => target.SetUser(It.Is<User>(user => user.Email == "a.pushkin@infinnity.ru" 
                                                                         && user.AccessToken == "access" 
                                                                         && user.RefreshToken == "refresh")), Times.Once);
    }
}