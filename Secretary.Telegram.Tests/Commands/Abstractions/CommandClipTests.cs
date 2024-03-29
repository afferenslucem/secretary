﻿using Moq;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Exceptions;

namespace Secretary.Telegram.Tests.Commands.Abstractions;

public class CommandClipTests
{
    private CommandClip _clip = null!;

    private Mock<Command> _firstState = null!; 
    private Mock<Command> _secondState = null!; 
    private Mock<Command> _thirdState = null!;
    private Mock<Command> _fourthState = null!;

    private CommandContext _context;

    [SetUp]
    public void Setup()
    {
        _firstState = new(); 
        _secondState = new(); 
        _thirdState = new(); 
        _fourthState = new(); 

        _firstState.Setup(target => target.OnMessage()).ReturnsAsync(1);
        _secondState.Setup(target => target.OnMessage()).ReturnsAsync(2);
        _thirdState.Setup(target => target.OnMessage()).ReturnsAsync(1);
        _fourthState.Setup(target => target.OnMessage()).ReturnsAsync(1);
        
        _clip = new(
            new List<Command>() {_firstState.Object, _secondState.Object, _thirdState.Object, _fourthState.Object}
            );

        _context = new()
        {
            UserMessage = new UserMessage()
        };
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
        
        // Инкремент вырос, потому что второе состоние вернуло 2, а не 1
        Assert.That(_clip.RunIndex, Is.EqualTo(3));
        
        _thirdState.Verify(target => target.Execute(), Times.Never);
        _fourthState.Verify(target => target.Execute(), Times.Once);
    }

    [Test]
    public async Task ShouldIgnoreExecuteOnLastStep()
    {
        await _clip.Run(_context);
        await _clip.Run(_context);
        await _clip.Run(_context);
        
        _fourthState.Verify(target => target.ValidateMessage(), Times.Once);
        _fourthState.Verify(target => target.OnMessage(), Times.Once);
        
        Assert.That(_clip.RunIndex, Is.EqualTo(4));
    }

    [Test]
    public async Task ShouldIgnoreStepIncrementForValidationError()
    {
        await _clip.Run(_context);
        
        _secondState.Setup(target => target.ValidateMessage()).ThrowsAsync(new IncorrectMessageException());
        
        Assert.That(_clip.RunIndex, Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldCancelCurrentChild()
    {
        await _clip.Run(_context);
        await _clip.Cancel(_context);
        
        _secondState.Verify(target => target.Cancel(), Times.Once);
    }

    [Test]
    public async Task ShouldIgnoreRunForFinish()
    {
        await _clip.Run(_context);
        await _clip.Run(_context);
        await _clip.Run(_context);
        
        Assert.That(_clip.RunIndex, Is.EqualTo(4));
        
        await _clip.Run(_context);

        Assert.That(_clip.RunIndex, Is.EqualTo(4));
    }

    [Test]
    public async Task ShouldReturnAsymmetricCompletedMarker()
    {
        var clip = new CommandClip(new[] { _firstState.Object, _secondState.Object, new AssymetricCompleteCommand() });

        await clip.Run(_context);
        
        Assert.That(clip.IsAsymmetricCompleted, Is.True);
        Assert.That(clip.IsCompleted, Is.True);
    }

    [Test]
    public async Task ShouldReturnFinishedMarker()
    {
        var clip = new CommandClip(new[] { _firstState.Object, _thirdState.Object });

        await clip.Run(_context);
        await clip.Run(_context);
        
        Assert.That(clip.IsFinishedChain, Is.True);
        Assert.That(clip.IsCompleted, Is.True);
    }
}