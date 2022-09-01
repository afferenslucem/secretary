using Moq;
using secretary.mail.Authentication;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.registermail;
using secretary.telegram.sessions;
using secretary.yandex.exceptions;

namespace secretary.telegram.tests.commands.registermail;

public class EnterCodeCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<IYandexAuthenticator> _yandexAuthenticator = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    private CommandContext _context = null!;
    private EnterCodeCommand _command = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();
        this._userStorage = new Mock<IUserStorage>();
        this._sessionStorage = new Mock<ISessionStorage>();
        this._yandexAuthenticator = new Mock<IYandexAuthenticator>();

        this._context = new CommandContext()
        {
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            YandexAuthenticator = _yandexAuthenticator.Object, 
            UserStorage = _userStorage.Object,
            SessionStorage = _sessionStorage.Object,
        };

        this._command = new EnterCodeCommand();
        
        this._command.Context = _context;
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
        
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, "Пожалуйста, <strong>УБЕДИТЕСЬ</strong>, что вы авторизуетесь в рабочей почте!\r\n" +
                                                              "Введите этот код: <code>code</code> в поле ввода по этой ссылке: url. Регистрация может занять пару минут."));
    }
        
    [Test]
    public async Task ShouldSendSorryForYandexGetAuthCode()
    {
        _yandexAuthenticator
            .Setup(target => target.GetAuthenticationCode(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new YandexAuthenticationException());
        
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, 
            "При запросе токена для авторизации произошла ошибка:(\r\n" +
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
        
        _yandexAuthenticator.Setup(target => target.CheckToken(It.IsAny<AuthenticationData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenData() { access_token = "token"});
        
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, "Ура, вы успешно зарегистрировали почту"));
    }
    
        
    [Test]
    public async Task ShouldUpdateTokens()
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
        
        await this._command.Execute();
        
        _userStorage.Verify(target => target.UpdateUser(It.Is<User>(
            user => user.ChatId == 2517 && user.Name == "Александр Пушкин" && user.AccessToken == "access_token" && user.RefreshToken == "refresh_token"
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
        
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, 
            "При запросе токена для авторизации произошла ошибка:(\r\n" +
            "Попробуйте через пару минут, если не сработает, то обратитесь по вот этому адресу @hrodveetnir"));
    }
}