using secretary.telegram.chains;
using secretary.telegram.commands;
using secretary.telegram.commands.registeruser;
using secretary.telegram.commands.timeoff;

namespace secretary.telegram.tests.chains;

public class CommandsListeningChainTests
{
    private CommandsListeningChain chain;
    
    [SetUp]
    public void Setup()
    {
        this.chain = new CommandsListeningChain();
    }

    [Test]
    public void ShouldReturnNullCommand()
    {
        var result = this.chain.Get("random string");
        
        Assert.IsInstanceOf<NullCommand>(result);
    }

    [Test]
    public void ShouldReturnTimeOffCommand()
    {
        var result = this.chain.Get(TimeOffCommand.Key);
        
        Assert.IsInstanceOf<TimeOffCommand>(result);
    }

    [Test]
    public void ShouldReturnRegisterMailCommand()
    {
        var result = this.chain.Get(RegisterMailCommand.Key);
        
        Assert.IsInstanceOf<RegisterMailCommand>(result);
    }

    [Test]
    public void ShouldReturnRegisterUserCommand()
    {
        var result = this.chain.Get(RegisterUserCommand.Key);
        
        Assert.IsInstanceOf<RegisterUserCommand>(result);
    }
}