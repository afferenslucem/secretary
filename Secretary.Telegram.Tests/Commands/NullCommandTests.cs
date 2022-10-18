using Moq;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Sessions;

namespace Secretary.Telegram.Tests.Commands;

public class NullCommandTests
{
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<ITelegramClient> _client = null!;
    
    private NullCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _sessionStorage = new Mock<ISessionStorage>();
        _client = new Mock<ITelegramClient>();

        _command = new NullCommand();
        
        _context = new CommandContext()
        { 
            UserMessage = new UserMessage { ChatId = 2517},
            SessionStorage = _sessionStorage.Object, 
            TelegramClient = _client.Object,
        };
        
        _command.Context = _context;
    }

    [Test]
    public void ShouldHaveCorrectKey()
    {
        Assert.That(NullCommand.Key, Is.EqualTo("*"));
    }
    
    [Test]
    public async Task ShouldReturnSorryForEmptySession()
    {
        _sessionStorage.Setup(target => target.GetSession(It.IsAny<long>())).ReturnsAsync((Session?)null);

        _command.Context = _context;
        
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Извините, я не понял\nОтправьте команду"));
    }
    
    [Test]
    public async Task ShouldRunLastCommand()
    {
        var lastCommand = new Mock<Command>();
        
        _sessionStorage
            .Setup(target => target.GetSession(It.IsAny<long>()))
            .ReturnsAsync(new Session(2517, lastCommand.Object));
        
        await _command.Execute();
        
        lastCommand.Verify(target => target.OnMessage());
    }
    
    [Test]
    public async Task ShouldRunLastCommandComplete()
    {
        var lastCommand = new Mock<Command>();
        
        _sessionStorage
            .Setup(target => target.GetSession(It.IsAny<long>()))
            .ReturnsAsync(new Session(2517, lastCommand.Object));
        
        await _command.Execute();
        
        lastCommand.Verify(target => target.OnComplete(), Times.Once);
    }

    [Test]
    public async Task ShouldNotDeleteSession()
    {
        await _command.OnComplete();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
    }
}