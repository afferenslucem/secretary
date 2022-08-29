using Moq;
using secretary.storage;
using secretary.telegram.commands;
using secretary.telegram.commands.registeruser;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands.regusteruser;

public class RegisterUserCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    private CommandContext _context = null!;
    private RegisterUserCommand _command = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();
        this._userStorage = new Mock<IUserStorage>();
        this._sessionStorage = new Mock<ISessionStorage>();

        this._context = new CommandContext()
        {
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            SessionStorage = _sessionStorage.Object, 
            UserStorage = _userStorage.Object,
        };

        this._command = new RegisterUserCommand();
    }

    [Test]
    public void ShouldCreate()
    {
        Assert.Pass();
    }
        
    [Test]
    public async Task ShouldSaveSessionOnExecute()
    {
        _sessionStorage.Setup(obj => obj.SaveSession(It.IsAny<long>(), It.IsAny<Session>()));

        await this._command.Execute(_context);
        
        this._sessionStorage.Verify(target => target.SaveSession(2517, It.Is<Session>(session => session.ChaitId == 2517 && session.LastCommand == _command)));
    }
}