using Moq;
using Secretary.Cache;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.RegisterUser;
using Secretary.Telegram.Sessions;

namespace Secretary.Telegram.Tests.Commands.RegisterUser;

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
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();
        _cacheService = new Mock<ICacheService>();
        _sessionStorage = new Mock<ISessionStorage>();

        _context = new CommandContext()
        {
            UserMessage = new UserMessage { ChatId = 2517},
            TelegramClient = _client.Object, 
            SessionStorage = _sessionStorage.Object, 
            UserStorage = _userStorage.Object,
            CacheService = _cacheService.Object,
        };

        _command = new RegisterUserCommand();
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
        Assert.That(RegisterUserCommand.Key, Is.EqualTo("/registeruser"));
    }
        
    [Test]
    public async Task ShouldSaveSessionOnExecute()
    {
        _sessionStorage.Setup(obj => obj.SaveSession(It.IsAny<long>(), It.IsAny<Session>()));

        await _command.Execute();
        
        _sessionStorage.Verify(target => target.SaveSession(2517, It.Is<Session>(session => session.ChatId == 2517 && session.LastCommand == _command)));
    }

    [Test]
    public async Task ShouldRunFully()
    {
        _cacheService.Setup(obj => obj.GetEntity<RegisterUserCache>(It.IsAny<long>())).ReturnsAsync(new RegisterUserCache());
        _userStorage.Setup(target => target.GetUser(2517)).ReturnsAsync(() => new User());
        
        _context.UserMessage.Text ="/registeruser";
        await _command.Execute();
        
        _context.UserMessage.Text ="Александр Пушкин";
        await _command.OnMessage();

        _context.UserMessage.Text ="Пушкина Александра Сергеевича";
        await _command.OnMessage();

        _context.UserMessage.Text ="поэт";
        await _command.OnMessage();
        
        var oldCache = new RegisterUserCache
        {
            Name = "Александр Пушкин",
            NameGenitive = "Пушкина Александра Сергеевича",
            JobTitle = "поэт"
        };
        
        _cacheService.Setup(obj => obj.GetEntity<RegisterUserCache>(It.IsAny<long>())).ReturnsAsync(oldCache);
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
        _context.UserMessage.Text ="поэта";
        
        await _command.OnMessage();
        await _command.OnComplete();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
        _userStorage.Verify(target => target.SetUser(It.Is<User>(user => user.JobTitleGenitive == "поэта")), Times.Once);
    }
}