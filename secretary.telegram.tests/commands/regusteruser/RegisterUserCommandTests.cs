using Moq;
using secretary.storage;
using secretary.storage.models;
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
        this._command.Context = _context;
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

        await this._command.Execute();
        
        this._sessionStorage.Verify(target => target.SaveSession(2517, It.Is<Session>(session => session.ChaitId == 2517 && session.LastCommand == _command)));
    }

    [Test]
    public async Task ShouldRunFully()
    {
        _userStorage.Setup(target => target.GetUser(2517)).ReturnsAsync(() => new User());
        
        _context.Message = "/registeruser";
        await _command.Execute();

        _context.Message = "Александр Пушкин";
        await _command.OnMessage();
        _userStorage.Verify(target => target.SetUser(It.Is<User>(user => user.Name == "Александр Пушкин")), Times.Once);

        _context.Message = "Пушкина Александра Сергеевича";
        await _command.OnMessage();
        _userStorage.Verify(target => target.UpdateUser(It.Is<User>(user => user.NameGenitive == "Пушкина Александра Сергеевича")), Times.Once);

        _context.Message = "поэт";
        await _command.OnMessage();
        _userStorage.Verify(target => target.UpdateUser(It.Is<User>(user => user.JobTitle == "поэт")), Times.Once);

        _context.Message = "поэта";
        await _command.OnMessage();
        _userStorage.Verify(target => target.UpdateUser(It.Is<User>(user => user.JobTitleGenitive == "поэта")), Times.Once);
    }
}