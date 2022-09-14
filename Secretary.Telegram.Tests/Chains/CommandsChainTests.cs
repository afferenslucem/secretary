using Secretary.Telegram.chains;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Factories;
using Secretary.Telegram.Commands.TimeOff;

namespace Secretary.Telegram.Tests.Chains;

public class CommandsChainTests
{
    private CommandsChain _chain = null!;
    
    [SetUp]
    public void Setup()
    {
        this._chain = new CommandsChain();
    }

    [Test]
    public void ShouldReturnNullCommandForEveryString()
    {
        this._chain.Add(NullCommand.Key, new CommandFactory<NullCommand>());
        this._chain.Add(TimeOffCommand.Key, new CommandFactory<TimeOffCommand>());
        
        var result = this._chain.Get("random string");
        Assert.IsInstanceOf<NullCommand>(result);
        
        
        result = this._chain.Get("another random string");
        Assert.IsInstanceOf<NullCommand>(result);
        
        
        result = this._chain.Get(TimeOffCommand.Key);
        Assert.IsInstanceOf<NullCommand>(result);
    }

    [Test]
    public void ShouldReturnTimeOffCommand()
    {
        this._chain.Add(TimeOffCommand.Key, new CommandFactory<TimeOffCommand>());
        this._chain.Add(NullCommand.Key, new CommandFactory<NullCommand>());
        
        var result = this._chain.Get(TimeOffCommand.Key);
        Assert.IsInstanceOf<TimeOffCommand>(result);
        
        result = this._chain.Get("random string");
        Assert.IsInstanceOf<NullCommand>(result);
    }
}