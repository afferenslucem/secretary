using Microsoft.Extensions.Logging;
using Moq;
using secretary.logging;
using secretary.telegram.commands;
using secretary.telegram.exceptions;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands;

public class CommandClipTests
{
    private ILogger<CommandClipTests> _logger = LogPoint.GetLogger<CommandClipTests>();
    
    private CommandClip _clip = null!;

    private Mock<Command> _firstState = null!; 
    private Mock<Command> _secondState = null!; 
    private Mock<Command> _thirdState = null!; 
    private Mock<Command> _command = null!;

    private CommandContext _context = new CommandContext();

    [SetUp]
    public void Setup()
    {
        _firstState = new(); 
        _secondState = new(); 
        _thirdState = new(); 
        _command = new(); 
        
        _clip = new(
            new List<Command>() {_firstState.Object, _secondState.Object, _thirdState.Object},
            _command.Object
            );
    }

    [Test]
    public void ShouldCreate()
    {
        Assert.Pass();
    }

    [Test]
    public async Task ShouldRunFirstState()
    {
        await _clip.Run(_context);
        
        _firstState.Verify(target => target.ValidateMessage(), Times.Once);
        _firstState.Verify(target => target.OnMessage(), Times.Once);
        
        Assert.That(_clip.RunIndex, Is.EqualTo(1));
        
        _secondState.Verify(target => target.Execute(), Times.Once);
    }

    [Test]
    public async Task ShouldRunSecondState()
    {
        await _clip.Run(_context);
        await _clip.Run(_context);
        
        _secondState.Verify(target => target.ValidateMessage(), Times.Once);
        _secondState.Verify(target => target.OnMessage(), Times.Once);
        
        Assert.That(_clip.RunIndex, Is.EqualTo(2));
        
        _thirdState.Verify(target => target.Execute(), Times.Once);
    }

    [Test]
    public async Task ShouldIgnoreExecuteOnLastStep()
    {
        await _clip.Run(_context);
        await _clip.Run(_context);
        await _clip.Run(_context);
        
        _thirdState.Verify(target => target.ValidateMessage(), Times.Once);
        _thirdState.Verify(target => target.OnMessage(), Times.Once);
        
        Assert.That(_clip.RunIndex, Is.EqualTo(3));
    }

    [Test]
    public async Task ShouldIgnoreStepIncrementForValidationError()
    {
        await _clip.Run(_context);
        
        _secondState.Setup(target => target.ValidateMessage()).ThrowsAsync(new IncorrectFormatException());
        
        Assert.That(_clip.RunIndex, Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldCancelCurrentChild()
    {
        await _clip.Run(_context);
        await _clip.Cancel(_context);
        
        _secondState.Verify(target => target.Cancel(), Times.Once);
    }
}