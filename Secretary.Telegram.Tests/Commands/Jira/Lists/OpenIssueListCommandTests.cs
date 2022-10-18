using Moq;
using Secretary.Cache;
using Secretary.JiraManager;
using Secretary.JiraManager.Data;
using Secretary.Storage.Interfaces;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Jira;
using Secretary.Telegram.Commands.Jira.Lists;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = Secretary.Storage.Models.User;

namespace Secretary.Telegram.Tests.Commands.Jira.Lists;

public class OpenIssueListCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IJiraReporter> _jiraReporter = null!;
    private Mock<IJiraReporterFactory> _jiraReporterFactory = null!;
    private Mock<ICacheService> _cacheService = null!;
    private Mock<IUserStorage> _userStorage = null!;

    private OpenIssuesListCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _jiraReporter = new Mock<IJiraReporter>();
        _jiraReporterFactory = new Mock<IJiraReporterFactory>();
        _cacheService = new Mock<ICacheService>();
        _userStorage = new Mock<IUserStorage>();

        _command = new OpenIssuesListCommand();

        _context = new CommandContext()
        {
            UserMessage = new UserMessage { ChatId = 2517, CallbackMessageId = 42},
            CacheService = _cacheService.Object,
            TelegramClient = _client.Object,
            UserStorage = _userStorage.Object,
        };

        _userStorage.Setup(target => target.GetUser(2517)).ReturnsAsync(new User { JiraPersonalAccessToken = "token" });

        _command.JiraReporterFactory = _jiraReporterFactory.Object;

        _jiraReporterFactory.Setup(target => target.Create(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(_jiraReporter.Object);

        _command.Context = _context;
    }

    [Test]
    public void ShouldHaveCorrectKey()
    {
        Assert.That(OpenIssuesListCommand.Key, Is.EqualTo("/todoissues"));
    }

    [Test]
    public async Task ShouldSendEmptyListPlaceholder()
    {
        _jiraReporter.Setup(target => target.GetMyToDoIssues(1, 5)).ReturnsAsync(new Page<IssueInfo>(1, 5, 0, new List<IssueInfo>()));

        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Здесь ничего нет"));
    }


    [Test]
    public async Task ShouldSendIssueInfo()
    {
        var data = new Page<IssueInfo>(
            1, 5, 23,
            new[]
            {
                new IssueInfo(),
                new IssueInfo(),
                new IssueInfo(),
                new IssueInfo(),
                new IssueInfo(),
            }
        );
        
        _jiraReporter.Setup(target => target.GetMyToDoIssues(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(data);
        var id = 0;

        _client
            .Setup(
                target => target.SendMessage(
                    2517, 
                    It.IsAny<string>(),
                    It.IsAny<InlineKeyboardMarkup>())
            )
            .ReturnsAsync(() => new Message() { MessageId = ++id });
        
        await _command.Execute();
        
        _client.Verify(
            target => target.SendMessage(
                2517, 
                It.IsAny<string>(),
                It.IsAny<InlineKeyboardMarkup>()
            ),
            Times.Exactly(6)
        );
        
        _cacheService.Verify(
            target => target.SaveEntity(
                2517,
                It.Is<OpenIssuesListNavigationCache>(cache => cache.MessageIds.SequenceEqual(new [] { 1, 2, 3, 4, 5})),
                It.IsAny<int>()
            )
        );
        
        _cacheService.Verify(
            target => target.SaveEntity(
                2517,
                It.Is<OpenIssuesListNavigationCache>(cache => cache.MenuMessageId == 6),
                It.IsAny<int>()
            )
        );
    }
}