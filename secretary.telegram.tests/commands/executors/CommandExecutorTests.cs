using Moq;
using secretary.telegram.commands;
using secretary.telegram.commands.executors;

namespace secretary.telegram.tests.commands.executors;

public class CommandExecutorTests
{
    [Test]
    public void ShouldSetContextToCommandOnCreate()
    {
        var command = new EmptyCommand();
        var context = new CommandContext();

        _ = new CommandExecutor(command, context);
        
        Assert.That(command.Context, Is.EqualTo(context));
    }

    [Test]
    public async Task ShouldRunExecute()
    {
        var commandMock = new Mock<Command>();
        var context = new CommandContext();

        await new CommandExecutor(commandMock.Object, context).Execute();
        
        commandMock.Verify(target => target.Execute(), Times.Once);
    }

    [Test]
    public async Task ShouldRunOnMessage()
    {
        var commandMock = new Mock<Command>();
        var context = new CommandContext();

        await new CommandExecutor(commandMock.Object, context).OnMessage();
        
        commandMock.Verify(target => target.OnMessage(), Times.Once);
    }

    [Test]
    public async Task ShouldRunValidate()
    {
        var commandMock = new Mock<Command>();
        var context = new CommandContext();

        await new CommandExecutor(commandMock.Object, context).ValidateMessage();
        
        commandMock.Verify(target => target.ValidateMessage(), Times.Once);
    }

    [Test]
    public async Task ShouldRunCancel()
    {
        var commandMock = new Mock<Command>();
        var context = new CommandContext();

        await new CommandExecutor(commandMock.Object, context).Cancel();
        
        commandMock.Verify(target => target.Cancel(), Times.Once);
    }

    [Test]
    public async Task ShouldRunOnComplete()
    {
        var commandMock = new Mock<Command>();
        var context = new CommandContext();

        await new CommandExecutor(commandMock.Object, context).OnComplete();
        
        commandMock.Verify(target => target.OnComplete(), Times.Once);
    }
}