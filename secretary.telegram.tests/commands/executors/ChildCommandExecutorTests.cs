using Moq;
using secretary.telegram.commands;
using secretary.telegram.commands.executors;

namespace secretary.telegram.tests.commands.executors;

public class ChildCommandExecutorTests
{
    [Test]
    public void ShouldSetContextToCommandOnCreate()
    {
        var command = new EmptyCommand();
        var context = new CommandContext();
        var parentCommand = new EmptyCommand();

        _ = new ChildCommandExecutor(command, context, parentCommand);
        
        Assert.That(command.Context, Is.EqualTo(context));
        Assert.That(command.ParentCommand, Is.EqualTo(parentCommand));
    }

    [Test]
    public async Task ShouldRunExecute()
    {
        var commandMock = new Mock<Command>();
        var context = new CommandContext();
        var parentCommand = new EmptyCommand();

        await new ChildCommandExecutor(commandMock.Object, context, parentCommand).Execute();
        
        commandMock.Verify(target => target.Execute(), Times.Once);
    }

    [Test]
    public async Task ShouldRunOnMessage()
    {
        var commandMock = new Mock<Command>();
        var context = new CommandContext();
        var parentCommand = new EmptyCommand();

        await new ChildCommandExecutor(commandMock.Object, context, parentCommand).OnMessage();
        
        commandMock.Verify(target => target.OnMessage(), Times.Once);
    }

    [Test]
    public async Task ShouldRunValidate()
    {
        var commandMock = new Mock<Command>();
        var context = new CommandContext();
        var parentCommand = new EmptyCommand();

        await new ChildCommandExecutor(commandMock.Object, context, parentCommand).ValidateMessage();
        
        commandMock.Verify(target => target.ValidateMessage(), Times.Once);
    }

    [Test]
    public async Task ShouldRunCancel()
    {
        var commandMock = new Mock<Command>();
        var context = new CommandContext();
        var parentCommand = new EmptyCommand();

        await new ChildCommandExecutor(commandMock.Object, context, parentCommand).Cancel();
        
        commandMock.Verify(target => target.Cancel(), Times.Once);
    }
}