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
    }

    [Test]
    public void ShouldCreate()
    {
        Assert.Pass();
    }

}