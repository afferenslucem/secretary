using Moq;
using secretary.cache;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.caches;
using secretary.telegram.commands.registeruser;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands.registeruser;

public class RegisterUserCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ICacheService> _cacheService = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    private CommandContext _context = null!;
    private RegisterUserCommand _command = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();
        this._userStorage = new Mock<IUserStorage>();
        this._cacheService = new Mock<ICacheService>();
        this._sessionStorage = new Mock<ISessionStorage>();

        this._context = new CommandContext()
        {
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            SessionStorage = _sessionStorage.Object, 
            UserStorage = _userStorage.Object,
            CacheService = _cacheService.Object,
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
    public void ShouldHaveCorrectKey()
    {
        Assert.That(RegisterUserCommand.Key, Is.EqualTo("/registeruser"));
    }
        
    [Test]
    public async Task ShouldSaveSessionOnExecute()
    {
        _sessionStorage.Setup(obj => obj.SaveSession(It.IsAny<long>(), It.IsAny<Session>()));

        await this._command.Execute();
        
        this._sessionStorage.Verify(target => target.SaveSession(2517, It.Is<Session>(session => session.ChatId == 2517 && session.LastCommand == _command)));
    }

    [Test]
    public async Task ShouldRunFully()
    {
        _cacheService.Setup(obj => obj.GetEntity<RegisterUserCache>(It.IsAny<long>())).ReturnsAsync(new RegisterUserCache());
        _userStorage.Setup(target => target.GetUser(2517)).ReturnsAsync(() => new User());
        
        _context.Message = "/registeruser";
        await _command.Execute();
        
        _context.Message = "Александр Пушкин";
        await _command.OnMessage();

        _context.Message = "Пушкина Александра Сергеевича";
        await _command.OnMessage();

        _context.Message = "поэт";
        await _command.OnMessage();
        
        var oldCache = new RegisterUserCache
        {
            Name = "Александр Пушкин",
            NameGenitive = "Пушкина Александра Сергеевича",
            JobTitle = "поэт"
        };
        
        _cacheService.Setup(obj => obj.GetEntity<RegisterUserCache>(It.IsAny<long>())).ReturnsAsync(oldCache);
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
        _context.Message = "поэта";
        
        await _command.OnMessage();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
        _userStorage.Verify(target => target.SetUser(It.Is<User>(user => user.JobTitleGenitive == "поэта")), Times.Once);
    }
}