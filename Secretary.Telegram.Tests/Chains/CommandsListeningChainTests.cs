using Secretary.Telegram.chains;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Distant;
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
        _chain = new CommandsListeningChain();
    }

    [Test]
    public void ShouldReturnNullCommand()
    {
        var result = _chain.Get("random string");
        
        Assert.IsInstanceOf<NullCommand>(result);
    }

    [Test]
    public void ShouldReturnStartCommand()
    {
        var result = _chain.Get("/start");
        
        Assert.IsInstanceOf<StartCommand>(result);
    }

    [Test]
    public void ShouldReturnSetEmailsCommand()
    {
        var result = _chain.Get("/setemails");
        
        Assert.IsInstanceOf<SetEmailsCommand>(result);
    }

    [Test]
    public void ShouldReturnCancelCommand()
    {
        var result = _chain.Get("/cancel");
        
        Assert.IsInstanceOf<CancelCommand>(result);
    }

    [Test]
    public void ShouldReturnTimeOffCommand()
    {
        var result = _chain.Get(TimeOffCommand.Key);
        
        Assert.IsInstanceOf<TimeOffCommand>(result);
    }

    [Test]
    public void ShouldReturnVacationCommand()
    {
        var result = _chain.Get(VacationCommand.Key);
        
        Assert.IsInstanceOf<VacationCommand>(result);
    }

    [Test]
    public void ShouldReturnRegisterMailCommand()
    {
        var result = _chain.Get(RegisterMailCommand.Key);
        
        Assert.IsInstanceOf<RegisterMailCommand>(result);
    }

    [Test]
    public void ShouldReturnRegisterUserCommand()
    {
        var result = _chain.Get(RegisterUserCommand.Key);
        
        Assert.IsInstanceOf<RegisterUserCommand>(result);
    }

    [Test]
    public void ShouldReturnMeCommand()
    {
        var result = _chain.Get(MeCommand.Key);
        
        Assert.IsInstanceOf<MeCommand>(result);
    }

    [Test]
    public void ShouldReturnDistantCommand()
    {
        var result = _chain.Get(DistantCommand.Key);
        
        Assert.IsInstanceOf<DistantCommand>(result);
    }

    [Test]
    public void ShouldReturnRemindLogTimeCommand()
    {
        var result = _chain.Get(RemindLogTimeCommand.Key);
        
        Assert.IsInstanceOf<RemindLogTimeCommand>(result);
    }
}