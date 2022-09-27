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
using Secretary.Telegram.Commands.Vacation;
using Secretary.Telegram.Sessions;
using Secretary.Telegram.Utils;
using Secretary.Yandex.Mail;

namespace Secretary.Telegram.Tests.Commands.Vacation;

public class VacationCommandTests
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

    private VacationCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();

        _sessionStorage = new Mock<ISessionStorage>();

        _userStorage = new Mock<IUserStorage>();
        _documentStorage = new Mock<IDocumentStorage>();
        _eventLogStorage = new Mock<IEventLogStorage>();
        _emailStorage = new Mock<IEmailStorage>();
        _mailClient = new Mock<IMailClient>();
        _cacheService = new Mock<ICacheService>();
        _fileManager = new Mock<IFileManager>();

        _command = new VacationCommand();
        
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
        
        _command.Context = _context;
        _command.FileManager = _fileManager.Object;
        
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(
            new User() { 
                Name = "Александр Пушкин",
                Email = "a.pushkin@infinnity.ru"
            }
        );
        
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
        Assert.That(VacationCommand.Key, Is.EqualTo("/vacation"));
    }
    
    [Test]
    public async Task ShouldSaveEventOnComplete()
    {
        var command = new Mock<VacationCommand>();
        command.CallBase = true;
        command.SetupGet(target => target.IsCompleted).Returns(true);
        
        command.Object.Context = _context;

        await command.Object.OnComplete();
        
        _eventLogStorage.Verify(target => target.Save(
            It.Is<EventLog>(log => log.Description == "Created Vacation")
        ));
        
        _eventLogStorage.Verify(target => target.Save(
            It.Is<EventLog>(log => log.UserChatId == 2517)
        ));
        
        _eventLogStorage.Verify(target => target.Save(
            It.Is<EventLog>(log => log.EventType == VacationCommand.Key)
        ));
    }
    
    [Test]
    public async Task ShouldDeleteSessionOnComplete()
    {
        var command = new Mock<VacationCommand>();
        command.CallBase = true;
        command.SetupGet(target => target.IsCompleted).Returns(true);
        
        command.Object.Context = _context;

        await command.Object.OnComplete();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517));
    }
    
    [Test]
    public async Task ShouldRemoveSessionForNo()
    {
        var cacheMock = new Mock<VacationCache>();
        cacheMock.SetupGet(target => target.DocumentKey).Returns("/vacation");
        cacheMock.SetupGet(target => target.Period).Returns(new DatePeriodParser().Parse("с 05.09.2022 по 18.09.2022"));
        
        _cacheService.Setup(target => target.GetEntity<VacationCache>(It.IsAny<long>())).ReturnsAsync(cacheMock.Object);

        
        _context.Message = "/vacation";
        await this._command.Execute();
        
        _context.Message = "с 05.09.2022 по 18.09.2022";
        await this._command.OnMessage();
        
        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), "/vacation"))
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
    public async Task ShouldExecuteReturnToEmailEnter()
    {
        var cacheMock = new Mock<VacationCache>();
        cacheMock.SetupGet(target => target.DocumentKey).Returns("/vacation");
        cacheMock.SetupGet(target => target.Period).Returns(new DatePeriodParser().Parse("05.09.2022"));
        
        _cacheService.Setup(target => target.GetEntity<VacationCache>(It.IsAny<long>())).ReturnsAsync(cacheMock.Object);
        
        _context.Message = "/vacation";
        await this._command.Execute();
        
        _context.Message = "с 05.09.2022 по 18.09.2022";
        await this._command.OnMessage();
        
        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), "/vacation"))
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
    public async Task ShouldCleanForceComplete()
    {
        _cacheService.Setup(target => target.GetEntity<VacationCache>(2517)).ReturnsAsync(
            new VacationCache
            {
                FilePath = "/vacation-path.docx"
            }
        );

        await _command.OnForceComplete();
        
        _fileManager.Verify(target => target.DeleteFile("/vacation-path.docx"));
        _cacheService.Verify(target => target.DeleteEntity<VacationCache>(2517));
        _sessionStorage.Verify(target => target.DeleteSession(2517));
    }
}