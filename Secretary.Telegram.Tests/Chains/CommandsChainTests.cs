using Secretary.Telegram.chains;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Documents.TimeOff;
using Secretary.Telegram.Commands.Factories;

namespace Secretary.Telegram.Tests.Chains;

public class CommandsChainTests
{
    private CommandsChain _chain = null!;
    
    [SetUp]
    public void Setup()
    {
        _chain = new CommandsChain();
    }

    [Test]
    public void ShouldReturnNullCommandForEveryString()
    {
        _chain.Add(NullCommand.Key, new CommandFactory<NullCommand>());
        _chain.Add(TimeOffCommand.Key, new CommandFactory<TimeOffCommand>());
        
        var result = _chain.Get("random string");
        Assert.IsInstanceOf<NullCommand>(result);
        
        
        result = _chain.Get("another random string");
        Assert.IsInstanceOf<NullCommand>(result);
        
        
        result = _chain.Get(TimeOffCommand.Key);
        Assert.IsInstanceOf<NullCommand>(result);
    }

    [Test]
    public void ShouldReturnTimeOffCommand()
    {
        _chain.Add(TimeOffCommand.Key, new CommandFactory<TimeOffCommand>());
        _chain.Add(NullCommand.Key, new CommandFactory<NullCommand>());
        
        var result = _chain.Get(TimeOffCommand.Key);
        Assert.IsInstanceOf<TimeOffCommand>(result);
        
        result = _chain.Get("random string");
        Assert.IsInstanceOf<NullCommand>(result);
    }
}