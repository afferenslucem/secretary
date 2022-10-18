using Moq;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Sessions;

namespace Secretary.Telegram.Tests.Commands;

public class CancelCommandTests
{
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<ITelegramClient> _client = null!;
    
    private CancelCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _sessionStorage = new Mock<ISessionStorage>();
        _client = new Mock<ITelegramClient>();

        _command = new CancelCommand();
        
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
        Assert.That(CancelCommand.Key, Is.EqualTo("/cancel"));
    }
    
    [Test]
    public async Task ShouldBreakCommand()
    {
        var commandMock = new Mock<Command>();
        var session = new Session(2517, commandMock.Object);
        _sessionStorage.Setup(target => target.GetSession(It.IsAny<long>())).ReturnsAsync(session);
        
        await _command.Execute();
        
        commandMock.Verify(target => target.OnForceComplete());
        _client.Verify(target => target.SendMessage(2517, "Дальнейшее выполнение команды прервано"));
    }
    
    [Test]
    public async Task ShouldCancelCommand()
    {
        var commandMock = new Mock<Command>();
        var session = new Session(2517, commandMock.Object);
        _sessionStorage.Setup(target => target.GetSession(It.IsAny<long>())).ReturnsAsync(session);
        
        await _command.Execute();
        
        commandMock.Verify(target => target.Cancel(), Times.Once);
    }
    
    [Test]
    public async Task ShouldNotDeleteSession()
    {
        await _command.OnComplete();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
    }
}