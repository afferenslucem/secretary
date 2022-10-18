using Moq;
using Secretary.JiraManager;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Jira.RegisterJiraToken;

namespace Secretary.Telegram.Tests.Commands.RegisterJiraToken;

public class RegisterJiraTokenActionCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<IJiraReporterFactory> _jiraReporterFactory = null!;
    private Mock<IJiraReporter> _jiraReporter = null!;
    private CommandContext _context = null!;
    private RegisterJiraTokenActionCommand _command = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();
        _jiraReporter = new Mock<IJiraReporter>();
        _jiraReporterFactory = new Mock<IJiraReporterFactory>();

        _context = new CommandContext()
        {
            UserMessage = new UserMessage { ChatId = 2517, MessageId = 42 },
            TelegramClient = _client.Object, 
            UserStorage = _userStorage.Object,
        };

        _command = new RegisterJiraTokenActionCommand();
        _command.JiraReporterFactory = _jiraReporterFactory.Object;

        _jiraReporterFactory.Setup(target => target.Create(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(_jiraReporter.Object);
        
        _command.Context = _context;
    }

    [Test]
    public async Task ShouldSendGreeting()
    {
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Отправьте свой Personal Access Token для JIRA"));
    }

    [Test]
    public async Task ShouldSendCheck()
    {
        _context.UserMessage.Text = "token";
        _jiraReporter.Setup(item => item.GetMyUsername()).ReturnsAsync("a.pushkin");
        
        await _command.OnMessage();
        
        _jiraReporterFactory.Verify(target => target.Create(It.IsAny<string>(), "token"), Times.Once);
        
        _client.Verify(target => target.SendMessage(2517, "Вы добавили токен для пользователя \"a.pushkin\""));
        _client.Verify(target => target.DeleteMessage(2517, 42));
    }

    [Test]
    public async Task ShouldSaveToken()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { Email = "a.pushkin@infinnity.ru" });

        _context.UserMessage.Text = "token";
        
        await _command.OnMessage();
        
        _userStorage.Verify(
            target => target.SetUser(
                It.Is<User>(user => user.Email == "a.pushkin@infinnity.ru" && user.JiraPersonalAccessToken == "token")
            )
        );
    }
}