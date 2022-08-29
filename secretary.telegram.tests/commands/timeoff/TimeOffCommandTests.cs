using Moq;
using secretary.storage;
using secretary.telegram.commands;
using secretary.telegram.commands.timeoff;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands.timeoff;

public class TimeOffCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<IUserStorage> _userStorage = null!;
    
    private TimeOffCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._sessionStorage = new Mock<ISessionStorage>();

        this._userStorage = new Mock<IUserStorage>();

        this._command = new TimeOffCommand();
        
        this._context = new CommandContext()
            { 
                ChatId = 2517, 
                TelegramClient = this._client.Object, 
                SessionStorage = _sessionStorage.Object, 
                UserStorage = _userStorage.Object
            };
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