using Secretary.Telegram.chains;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.RegisterMail;
using Secretary.Telegram.Commands.RegisterUser;
using Secretary.Telegram.Commands.SetEmails;
using Secretary.Telegram.Commands.TimeOff;
using Secretary.Telegram.Commands.Vacation;

namespace Secretary.Telegram.Tests.Chains;

public class CommandsListeningChainTests
{
    private CommandsListeningChain _chain = null!;
    
    [SetUp]
    public void Setup()
    {
        this._chain = new CommandsListeningChain();
    }

    [Test]
    public void ShouldReturnNullCommand()
    {
        var result = this._chain.Get("random string");
        
        Assert.IsInstanceOf<NullCommand>(result);
    }

    [Test]
    public void ShouldReturnStartCommand()
    {
        var result = this._chain.Get("/start");
        
        Assert.IsInstanceOf<StartCommand>(result);
    }

    [Test]
    public void ShouldReturnSetEmailsCommand()
    {
        var result = this._chain.Get("/setemails");
        
        Assert.IsInstanceOf<SetEmailsCommand>(result);
    }

    [Test]
    public void ShouldReturnCancelCommand()
    {
        var result = this._chain.Get("/cancel");
        
        Assert.IsInstanceOf<CancelCommand>(result);
    }

    [Test]
    public void ShouldReturnTimeOffCommand()
    {
        var result = this._chain.Get(TimeOffCommand.Key);
        
        Assert.IsInstanceOf<TimeOffCommand>(result);
    }

    [Test]
    public void ShouldReturnVacationCommand()
    {
        var result = this._chain.Get(VacationCommand.Key);
        
        Assert.IsInstanceOf<VacationCommand>(result);
    }

    [Test]
    public void ShouldReturnRegisterMailCommand()
    {
        var result = this._chain.Get(RegisterMailCommand.Key);
        
        Assert.IsInstanceOf<RegisterMailCommand>(result);
    }

    [Test]
    public void ShouldReturnRegisterUserCommand()
    {
        var result = this._chain.Get(RegisterUserCommand.Key);
        
        Assert.IsInstanceOf<RegisterUserCommand>(result);
    }

    [Test]
    public void ShouldReturnMeCommand()
    {
        var result = this._chain.Get(MeCommand.Key);
        
        Assert.IsInstanceOf<MeCommand>(result);
    }

    [Test]
    public void ShouldReturnVersionCommand()
    {
        var result = this._chain.Get(VersionCommand.Key);
        
        Assert.IsInstanceOf<VersionCommand>(result);
    }
}