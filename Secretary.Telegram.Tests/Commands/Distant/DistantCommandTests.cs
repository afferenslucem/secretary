using Moq;
using Secretary.Cache;
using Secretary.Configuration;
using Secretary.Documents;
using Secretary.Documents.utils;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Distant;
using Secretary.Telegram.Commands.TimeOff;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Sessions;
using Secretary.Telegram.Utils;
using Secretary.Yandex.Mail;

namespace Secretary.Telegram.Tests.Commands.Distant;

public class DistantCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<IDocumentStorage> _documentStorage = null!;
    private Mock<IEmailStorage> _emailStorage = null!;
    private Mock<IEventLogStorage> _eventLogStorage = null!;
    private Mock<IMailClient> _mailClient = null!;
    private Mock<ICacheService> _cacheService = null!;
    private Mock<IFileManager> _fileManager = null!;

    private DistantCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _sessionStorage = new Mock<ISessionStorage>();
        _userStorage = new Mock<IUserStorage>();
        _documentStorage = new Mock<IDocumentStorage>();
        _emailStorage = new Mock<IEmailStorage>();
        _eventLogStorage = new Mock<IEventLogStorage>();
        _mailClient = new Mock<IMailClient>();
        _cacheService = new Mock<ICacheService>();
        _fileManager = new Mock<IFileManager>();

        _command = new DistantCommand();
        _command.FileManager = _fileManager.Object;
        
        _context = new CommandContext()
            { 
                ChatId = 2517, 
                TelegramClient = _client.Object, 
                SessionStorage = _sessionStorage.Object, 
                UserStorage = _userStorage.Object,
                DocumentStorage = _documentStorage.Object,
                EmailStorage = _emailStorage.Object,
                EventLogStorage = _eventLogStorage.Object,
                MailClient = _mailClient.Object,
                CacheService = _cacheService.Object,
            };
        
        this._command.Context = _context;
        
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { JobTitleGenitive = "", AccessToken = ""});
        
        DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);
    }
    
    [Test]
    public void ShouldCreate()
    {
        Assert.Pass();
    }

    [Test]
    public void ShouldHaveCorrectKey()
    {
        Assert.That(DistantCommand.Key, Is.EqualTo("/distant"));
    }
    
    [Test]
    public async Task ShouldSaveSessionOnExecute()
    {
        _sessionStorage.Setup(obj => obj.SaveSession(It.IsAny<long>(), It.IsAny<Session>()));

        await this._command.Execute();
        
        this._sessionStorage.Verify(target => target.SaveSession(2517, It.Is<Session>(session => session.ChatId == 2517 && session.LastCommand == _command)));
    }
    
    [Test]
    public async Task ShouldSaveEventOnComplete()
    {
        var command = new Mock<DistantCommand>();
        command.CallBase = true;
        command.SetupGet(target => target.IsCompleted).Returns(true);
        
        command.Object.Context = _context;

        await command.Object.OnComplete();
        
        _eventLogStorage.Verify(target => target.Save(
            It.Is<EventLog>(log => log.Description == "Created Distant")
        ));
        
        _eventLogStorage.Verify(target => target.Save(
            It.Is<EventLog>(log => log.UserChatId == 2517)
        ));
        
        _eventLogStorage.Verify(target => target.Save(
            It.Is<EventLog>(log => log.EventType == DistantCommand.Key)
        ));
    }
    
    [Test]
    public async Task ShouldDeleteSessionOnComplete()
    {
        var command = new Mock<DistantCommand>();
        command.CallBase = true;
        command.SetupGet(target => target.IsCompleted).Returns(true);
        
        command.Object.Context = _context;

        await command.Object.OnComplete();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517));
    }

    [Test]
    public void ShouldBreakExecutionForNonRegisteredUser()
    {
        _context.Message = "/distant";

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);
        
        Assert.ThrowsAsync<NonCompleteUserException>(() => this._command.Execute());
        
        _client.Verify(target => target.SendMessage(2517, "Вы – незарегистрированный пользователь.\n\n" +
                                                          "Выполните команды:\n" +
                                                          "/registeruser\n" +
                                                          "/registermail"));
    }

    [Test]
    public void ShouldBreakExecutionForUserWithoutMail()
    {
        _context.Message = "/distant";

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { JobTitleGenitive = ""});
        
        Assert.ThrowsAsync<NonCompleteUserException>(() => this._command.Execute());
        
        _client.Verify(target => target.SendMessage(2517, "У вас не зарегистрирована почта.\n" +
                                                          "Выполните команду: /registermail"));
    }

    [Test]
    public void ShouldBreakExecutionForUserWithoutInfo()
    {
        _context.Message = "/distant";

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { AccessToken = ""});
        
        Assert.ThrowsAsync<NonCompleteUserException>(() => this._command.Execute());
        
        _client.Verify(target => target.SendMessage(2517, 
            "У вас не заданы данные о пользователе.\n" +
            "Выполните команду /registeruser"));
    }
    
    [Test]
    public async Task ShouldExecuteSkipCheckEmailsForRepeat()
    {
        _cacheService.Setup(target => target.GetEntity<DistantCache>(It.IsAny<long>())).ReturnsAsync(new DistantCache()
        {
            Period = new DatePeriodParser().Parse("05.09.2022"),
            Reason = "плохое самочувствие",
        });

        
        _context.Message = "/distant";
        await this._command.Execute();
        
        _context.Message = "30.08.2022";
        await this._command.OnMessage();
        
        _context.Message = "плохое самочувствие";
        await this._command.OnMessage();
        _client.Verify(target => target.SendDocument(2517, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document(1, ""));
        _emailStorage
            .Setup(target => target.GetForDocument(It.IsAny<long>()))
            .ReturnsAsync(new [] { new Email("a.pushkin@infinnity.ru") });
        _context.Message = "Да";
        await this._command.OnMessage();
        await _command.OnComplete();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
        _context.Message = "Повторить";
        await this._command.OnMessage();
        _mailClient.Verify(target => target.SendMail(It.IsAny<SecretaryMailMessage>()), Times.Once);
    }
    
    [Test]
    public async Task ShouldExecuteShowCheckEmailsForNewEmail()
    {
        _cacheService.Setup(target => target.GetEntity<DistantCache>(It.IsAny<long>())).ReturnsAsync(new DistantCache()
        {
            Period = new DatePeriodParser().Parse("05.09.2022"),
            Reason = "плохое самочувствие",
        });

        _context.Message = "/distant";
        await this._command.Execute();
        
        _context.Message = "30.08.2022";
        await this._command.OnMessage();
        
        _context.Message = "плохое самочувствие";
        await this._command.OnMessage();
        _client.Verify(target => target.SendDocument(2517, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document(1, ""));
        _emailStorage
            .Setup(target => target.GetForDocument(It.IsAny<long>()))
            .ReturnsAsync(new [] { new Email("a.pushkin@infinnity.ru") });
        _context.Message = "Да";
        await this._command.OnMessage();

        _context.Message = "a.pushkin@infinnity.ru";
        await this._command.OnMessage();

        _context.Message = "верно";
        await this._command.OnMessage();
        
        _mailClient.Verify(target => target.SendMail(It.IsAny<SecretaryMailMessage>()), Times.Once);
    }
    
    [Test]
    public async Task ShouldExecuteReturnToEmailEnter()
    {
        _cacheService.Setup(target => target.GetEntity<DistantCache>(It.IsAny<long>())).ReturnsAsync(new DistantCache()
        {
            Period = new DatePeriodParser().Parse("05.09.2022"),
            Reason = "плохое самочувствие",
        });

        
        _context.Message = "/distant";
        await this._command.Execute();
        
        _context.Message = "30.08.2022";
        await this._command.OnMessage();
        
        _context.Message = "плохое самочувствие";
        await this._command.OnMessage();
        _client.Verify(target => target.SendDocument(2517, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document(1, ""));
        _emailStorage
            .Setup(target => target.GetForDocument(It.IsAny<long>()))
            .ReturnsAsync(new [] { new Email("a.pushkin@infinnity.ru") });
        _context.Message = "Да";
        await this._command.OnMessage();

        _context.Message = "a.pushkin@infinnity.ru";
        await this._command.OnMessage();

        _context.Message = "не верно";
        await this._command.OnMessage();
        
        _context.Message = "a.pushkin@infinnity.ru";
        await this._command.OnMessage();

        _context.Message = "верно";
        await this._command.OnMessage();
        
        _mailClient.Verify(target => target.SendMail(It.IsAny<SecretaryMailMessage>()), Times.Once);
    }
    
    [Test]
    public async Task ShouldRemoveSessionForNo()
    {
        _cacheService.Setup(target => target.GetEntity<DistantCache>(It.IsAny<long>())).ReturnsAsync(new DistantCache()
        {
            Period = new DatePeriodParser().Parse("05.09.2022"),
            Reason = "плохое самочувствие",
        });

        
        _context.Message = "/distant";
        await this._command.Execute();
        
        _context.Message = "30.08.2022";
        await this._command.OnMessage();
        
        _context.Message = "плохое самочувствие";
        await this._command.OnMessage();
        _client.Verify(target => target.SendDocument(2517, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document(1, ""));
        _emailStorage
            .Setup(target => target.GetForDocument(It.IsAny<long>()))
            .ReturnsAsync(new [] { new Email("a.pushkin@infinnity.ru") });
        
        _context.Message = "Нет";
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
        await this._command.OnMessage();
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
    }

    [Test]
    public async Task ShouldCleanForceComplete()
    {
        _cacheService.Setup(target => target.GetEntity<DistantCache>(2517)).ReturnsAsync(
            new DistantCache
            {
                FilePath = "/distant-path.docx"
            }
        );

        await _command.OnForceComplete();
        
        _fileManager.Verify(target => target.DeleteFile("/distant-path.docx"));
        _cacheService.Verify(target => target.DeleteEntity<DistantCache>(2517));
        _sessionStorage.Verify(target => target.DeleteSession(2517));
    }
}