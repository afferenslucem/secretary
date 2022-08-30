﻿using Moq;
using secretary.mail.Authentication;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.registermail;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands.registermail;

public class RegisterMailCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage= null!;
    private Mock<IYandexAuthenticator> _mailClient= null!;
    private Mock<ISessionStorage> _sessionStorage= null!;
    private CommandContext _context= null!;
    private RegisterMailCommand _command= null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();
        this._userStorage = new Mock<IUserStorage>();
        this._sessionStorage = new Mock<ISessionStorage>();
        this._mailClient = new Mock<IYandexAuthenticator>();

        this._context = new CommandContext()
        {
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            YandexAuthenticator = _mailClient.Object, 
            UserStorage = _userStorage.Object,
            SessionStorage = _sessionStorage.Object, 
        };

        this._command = new RegisterMailCommand();
    }

    [Test]
    public void ShouldCreate()
    {
        Assert.Pass();
    }


    [Test]
    public async Task ShouldRunFully()
    {
        _userStorage.Setup(target => target.GetUser(2517)).ReturnsAsync(() => new User());
        
        _context.Message = "/registermail";
        await _command.Execute(_context);

        _mailClient
            .Setup(target => target.GetAuthenticationCode(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthenticationData() { expires_in = 300 } );
        _mailClient
            .Setup(target => target.CheckToken(It.IsAny<AuthenticationData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenData() { access_token = "access", refresh_token = "refresh"});
        
        _context.Message = "a.pushkin@infinnity.ru";
        await _command.OnMessage(_context);
        _userStorage.Verify(target => target.SetUser(It.Is<User>(user => user.Email == "a.pushkin@infinnity.ru")), Times.Once);
        _userStorage.Verify(target => target.UpdateUser(It.Is<User>(user => user.AccessToken == "access" && user.RefreshToken == "refresh")), Times.Once);
    }
}