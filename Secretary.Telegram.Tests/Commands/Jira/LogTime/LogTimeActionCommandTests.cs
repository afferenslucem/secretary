using Moq;
using Secretary.Cache;
using Secretary.JiraManager;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Jira;
using Secretary.Telegram.Commands.Jira.LogTime;

namespace Secretary.Telegram.Tests.Commands.Jira.LogTime;

public class LogTimeActionCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ICacheService> _cacheService = null!;
    private Mock<IJiraReporter> _jiraReporter = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<IJiraReporterFactory> _jiraReporterFactory = null!;
    
    private LogTimeActionCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _cacheService = new Mock<ICacheService>();
        _jiraReporter = new Mock<IJiraReporter>();
        _jiraReporterFactory = new Mock<IJiraReporterFactory>();
        _userStorage = new Mock<IUserStorage>();
       
        _command = new LogTimeActionCommand();

        _context = new CommandContext()
        { 
            UserMessage = new UserMessage(2517, "pushkin", "/logtime MES-1234"),
            TelegramClient = _client.Object, 
            CacheService = _cacheService.Object,
            UserStorage = _userStorage.Object,
        };
        
        _command.Context = _context;
        _command.JiraReporterFactory = _jiraReporterFactory.Object;
        _jiraReporterFactory.Setup(target => target.Create(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(_jiraReporter.Object);
    }

    [Test]
    public async Task ShouldSaveCache()
    {
        await _command.Execute();
        
        _cacheService.Verify(
            target => target.SaveEntity(2517, 
                It.Is<LogTimeCache>(cache => cache.IssueKey == "MES-1234"), 
                It.IsAny<int>()));
    }

    [Test]
    public async Task ShouldSendMessage()
    {
        await _command.Execute();
        
        _client.Verify(
            target => target.SendMessage(2517, 
                $"Логгирование времени в задачу <a href=\"https://jira.pushkin.ru/browse/MES-1234\">MES-1234</a>\n\n" +
                $"Введите время в формате <i>d</i>h <i>dd</i>m для логгирования с минутами\n" +
                $"Пример: <i>1h 30m</i>\n\n" +
                $"Введите просто число для логгирования часов\n" +
                $"Пример: <i>1</i>"
            )
        );
    }

    [Test]
    public void ShouldReadHours()
    {
        _context.UserMessage.Text = "8";

        var result = _command.ReadTime();
        
        Assert.That(result, Is.EqualTo("8h"));
    }

    [Test]
    public void ShouldReadHoursAndMinutes()
    {
        _context.UserMessage.Text = "1h 30m";

        var result = _command.ReadTime();
        
        Assert.That(result, Is.EqualTo("1h 30m"));
    }

    [Test]
    public async Task ShouldLogTime()
    {
        _context.UserMessage.Text = "1";
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>()))
            .ReturnsAsync(new User { JiraPersonalAccessToken = "token" });

        _cacheService.Setup(tar => tar.GetEntity<LogTimeCache>(2517))
            .ReturnsAsync(new LogTimeCache() { IssueKey = "MES-1234" });
        
        await _command.OnMessage();
        
        _jiraReporter.Verify(target => target.LogTime("MES-1234", "1h"));
    }

    [Test]
    public async Task ShouldSendDone()
    {
        _context.UserMessage.Text = "1";
        
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>()))
            .ReturnsAsync(new User { JiraPersonalAccessToken = "token" });

        _cacheService.Setup(tar => tar.GetEntity<LogTimeCache>(2517))
            .ReturnsAsync(new LogTimeCache() { IssueKey = "MES-1234" });

        await _command.OnMessage();
        
        _client.Verify(target => target.SendMessage(2517, "Вы залоггировали 1h в <a href=\"https://jira.pushkin.ru/browse/MES-1234\">MES-1234</a>"));
    }
}