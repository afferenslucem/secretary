using Moq;
using Secretary.Cache;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Sessions;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Exceptions;

namespace Secretary.Telegram.Tests.Commands;

public class RenewTokenCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<IYandexAuthenticator> _yandexAuthenticator = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    
    private RenewTokenCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();
        _sessionStorage = new Mock<ISessionStorage>();
        _yandexAuthenticator = new Mock<IYandexAuthenticator>();

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User()
        {
            JobTitleGenitive = "",
            AccessToken = ""
        });

        _context = new CommandContext()
        {
            ChatId = 2517, 
            TelegramClient = _client.Object, 
            YandexAuthenticator = _yandexAuthenticator.Object, 
            UserStorage = _userStorage.Object,
            SessionStorage = _sessionStorage.Object,
        };
        
        _command = new RenewTokenCommand();

        _command.Context = _context;
    }

    [Test]
    public void ShouldHaveCorrectKey()
    {
        Assert.That(RenewTokenCommand.Key, Is.EqualTo("/renewtoken"));
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
            Name = "Александр Пушкин",
            AccessToken = "",
            JobTitleGenitive = "",
        };

        _userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);
        
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
            Name = "Александр Пушкин",
            Email = "a.pushkin@infinnity.ru",
            AccessToken = "",
            JobTitleGenitive = "",
        };
        _userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);
        
        await _command.Execute();

        _userStorage.Verify(target => target.SetUser(It.Is<User>(
            user => user.ChatId == 2517
                    && user.Name == "Александр Пушкин"
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
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredUser()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);

        Assert.ThrowsAsync<NonCompleteUserException>(() => _command.Execute());
    }
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredMail()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { JobTitleGenitive = "" });

        Assert.ThrowsAsync<NonCompleteUserException>(() => _command.Execute());
    }
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredPersonalInfo()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { AccessToken = "" });

        Assert.ThrowsAsync<NonCompleteUserException>(() => _command.Execute());
    }
}