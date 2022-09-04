using Moq;
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
        
        this._command.Context = _context;
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
        _userStorage.Setup(target => target.GetUser(2517)).ReturnsAsync(() => new User());
        
        _context.Message = "/registermail";
        await _command.Execute();
        
        _mailClient
            .Setup(target => target.GetAuthenticationCode(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthenticationData() { expires_in = 300 } );
        _mailClient
            .Setup(target => target.CheckToken(It.IsAny<AuthenticationData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenData() { access_token = "access", refresh_token = "refresh"});
        
        _context.Message = "a.pushkin@infinnity.ru";
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
        
        await _command.OnMessage();
        
        _userStorage.Verify(target => target.SetUser(It.Is<User>(user => user.Email == "a.pushkin@infinnity.ru")), Times.Once);
        _userStorage.Verify(target => target.UpdateUser(It.Is<User>(user => user.AccessToken == "access" && user.RefreshToken == "refresh")), Times.Once);
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
    }
    
    [Test]
    public async Task ShouldAskEmailAgainForIncorrectEmail()
    {
        _userStorage.Setup(target => target.GetUser(2517)).ReturnsAsync(() => new User());
        
        _context.Message = "/registermail";
        await _command.Execute();

        _context.Message = "a.pushkin";

        await _command.OnMessage();
        _client.Verify(target => target.SendMessage(2517, "Некорректный формат почты. Введите почту еще раз"));
    }
    
    [Test]
    public async Task ShouldPassExecutionAfterIncorrectMail()
    {
        _userStorage.Setup(target => target.GetUser(2517)).ReturnsAsync(() => new User());
        
        _context.Message = "/registermail";
        await _command.Execute();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);

        _context.Message = "a.pushkin";

        await _command.OnMessage();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
        _client.Verify(target => target.SendMessage(2517, "Некорректный формат почты. Введите почту еще раз"));
        
        _mailClient
            .Setup(target => target.GetAuthenticationCode(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthenticationData() { expires_in = 300 } );
        _mailClient
            .Setup(target => target.CheckToken(It.IsAny<AuthenticationData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenData() { access_token = "access", refresh_token = "refresh"});
        
        _context.Message = "a.pushkin@infinnity.ru";
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
        await _command.OnMessage();
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
        _userStorage.Verify(target => target.SetUser(It.Is<User>(user => user.Email == "a.pushkin@infinnity.ru")), Times.Once);
        _userStorage.Verify(target => target.UpdateUser(It.Is<User>(user => user.AccessToken == "access" && user.RefreshToken == "refresh")), Times.Once);
    }
}