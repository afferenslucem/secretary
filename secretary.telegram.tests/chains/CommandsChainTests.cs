using secretary.telegram.chains;
using secretary.telegram.commands;
using secretary.telegram.commands.factories;
using secretary.telegram.commands.timeoff;

namespace secretary.telegram.tests.chains;

public class CommandsChainTests
{
    private CommandsChain chain;
    
    [SetUp]
    public void Setup()
    {
        this.chain = new CommandsChain();
    }

    [Test]
    public void ShouldReturnNullCommandForEveryString()
    {
        this.chain.Add(NullCommand.Key, new NullCommandFactory());
        this.chain.Add(TimeOffCommand.Key, new TimeOffCommandFactory());
        
        var result = this.chain.Get("random string");
        Assert.IsInstanceOf<NullCommand>(result);
        
        
        result = this.chain.Get("another random string");
        Assert.IsInstanceOf<NullCommand>(result);
        
        
        result = this.chain.Get(TimeOffCommand.Key);
        Assert.IsInstanceOf<NullCommand>(result);
    }

    [Test]
    public void ShouldReturnTimeOffCommand()
    {
        this.chain.Add(TimeOffCommand.Key, new TimeOffCommandFactory());
        this.chain.Add(NullCommand.Key, new NullCommandFactory());
        
        var result = this.chain.Get(TimeOffCommand.Key);
        Assert.IsInstanceOf<TimeOffCommand>(result);
        
        result = this.chain.Get("random string");
        Assert.IsInstanceOf<NullCommand>(result);
    }
}