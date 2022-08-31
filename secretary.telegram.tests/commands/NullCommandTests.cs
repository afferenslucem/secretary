using Moq;
using secretary.telegram.commands;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands;

public class NullCommandTests
{
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<ITelegramClient> _client = null!;
    
    private NullCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._sessionStorage = new Mock<ISessionStorage>();
        this._client = new Mock<ITelegramClient>();

        this._command = new NullCommand();
        
        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            SessionStorage = this._sessionStorage.Object, 
            TelegramClient = this._client.Object,
        };
        
        this._command.Context = _context;
    }
    
    [Test]
    public async Task ShouldReturnSorryForEmptySession()
    {
        _sessionStorage.Setup(target => target.GetSession(It.IsAny<long>())).ReturnsAsync((Session?)null);

        _command.Context = _context;
        
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, "Извините, я не понял\r\nОтправьте команду"));
    }
    
    [Test]
    public async Task ShouldReturnSorryForLastCommand()
    {
        _sessionStorage.Setup(target => target.GetSession(It.IsAny<long>())).ReturnsAsync(new Session());
        
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, "Извините, я не понял\r\nОтправьте команду"));
    }
    
    [Test]
    public async Task ShouldRunLastCommand()
    {
        var lastCommand = new Mock<Command>();
        
        _sessionStorage
            .Setup(target => target.GetSession(It.IsAny<long>()))
            .ReturnsAsync(new Session() { LastCommand = lastCommand.Object});
        
        await this._command.Execute();
        
        lastCommand.Verify(target => target.OnMessage());
    }
}