using Secretary.Telegram.chains;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Documents;
using Secretary.Telegram.Commands.Documents.Distant;
using Secretary.Telegram.Commands.Documents.SetEmails;
using Secretary.Telegram.Commands.Documents.TimeOff;
using Secretary.Telegram.Commands.Documents.Vacation;
using Secretary.Telegram.Commands.Jira;
using Secretary.Telegram.Commands.Jira.Lists;
using Secretary.Telegram.Commands.Jira.RegisterJiraToken;
using Secretary.Telegram.Commands.Menus;
using Secretary.Telegram.Commands.RegisterMail;
using Secretary.Telegram.Commands.RegisterUser;

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
    public void ShouldReturnLogTimeCommand()
    {
        var result = _chain.Get(RegisterUserCommand.Key);
        
        Assert.IsInstanceOf<RegisterUserCommand>(result);
    }

    [Test]
    public void ShouldReturnRenewTokenCommand()
    {
        var result = _chain.Get(RenewTokenCommand.Key);
        
        Assert.IsInstanceOf<RenewTokenCommand>(result);
    }

    [Test]
    public void ShouldReturnJiraCommand()
    {
        var result = _chain.Get(JiraMenuCommand.Key);
        
        Assert.IsInstanceOf<JiraMenuCommand>(result);
    }

    [Test]
    public void ShouldReturnOpenIssuesCommand()
    {
        var result = _chain.Get(OpenIssuesListCommand.Key);
        
        Assert.IsInstanceOf<OpenIssuesListCommand>(result);
    }

    [Test]
    public void ShouldReturnDayTimeCommandCommand()
    {
        var result = _chain.Get(DayReportCommand.Key);
        
        Assert.IsInstanceOf<DayReportCommand>(result);
    }

    [Test]
    public void ShouldReturnRegisterJiraTokenCommandCommand()
    {
        var result = _chain.Get(RegisterJiraTokenCommand.Key);
        
        Assert.IsInstanceOf<RegisterJiraTokenCommand>(result);
    }

    [Test]
    public void ShouldReturnProgressIssuesCommand()
    {
        var result = _chain.Get(ProgressIssuesListCommand.Key);
        
        Assert.IsInstanceOf<ProgressIssuesListCommand>(result);
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
    public void ShouldReturnWeekReportCommand()
    {
        var result = _chain.Get(WeekReportCommand.Key);
        
        Assert.IsInstanceOf<WeekReportCommand>(result);
    }

    [Test]
    public void ShouldReturnEmptyCommand()
    {
        var result = _chain.Get(EmptyCommand.Key);
        
        Assert.IsInstanceOf<EmptyCommand>(result);
    }

    [Test]
    public void ShouldReturnRemindLogTimeCommand()
    {
        var result = _chain.Get(RemindLogTimeCommand.Key);
        
        Assert.IsInstanceOf<RemindLogTimeCommand>(result);
    }

    [Test]
    public void ShouldReturnSendDocumentRedirectCommandCommand()
    {
        var result = _chain.Get(SendDocumentRedirectCommand.Key);
        
        Assert.IsInstanceOf<SendDocumentRedirectCommand>(result);
    }
}