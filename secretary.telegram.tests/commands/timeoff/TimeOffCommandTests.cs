using Moq;
using secretary.configuration;
using secretary.documents;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.timeoff;
using secretary.telegram.sessions;
using secretary.yandex.mail;

namespace secretary.telegram.tests.commands.timeoff;

public class TimeOffCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<IDocumentStorage> _documentStorage = null!;
    private Mock<IEmailStorage> _emailStorage = null!;
    private Mock<IMailClient> _mailClient = null!;
    
    private TimeOffCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._sessionStorage = new Mock<ISessionStorage>();

        this._userStorage = new Mock<IUserStorage>();
        this._documentStorage = new Mock<IDocumentStorage>();
        this._emailStorage = new Mock<IEmailStorage>();
        this._mailClient = new Mock<IMailClient>();

        this._command = new TimeOffCommand();
        
        this._context = new CommandContext()
            { 
                ChatId = 2517, 
                TelegramClient = _client.Object, 
                SessionStorage = _sessionStorage.Object, 
                UserStorage = _userStorage.Object,
                DocumentStorage = _documentStorage.Object,
                EmailStorage = _emailStorage.Object,
                MailClient = _mailClient.Object,
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
        Assert.That(TimeOffCommand.Key, Is.EqualTo("/timeoff"));
    }
    
    [Test]
    public async Task ShouldSaveSessionOnExecute()
    {
        _sessionStorage.Setup(obj => obj.SaveSession(It.IsAny<long>(), It.IsAny<Session>()));

        await this._command.Execute();
        
        this._sessionStorage.Verify(target => target.SaveSession(2517, It.Is<Session>(session => session.ChaitId == 2517 && session.LastCommand == _command)));
    }

    [Test]
    public async Task ShouldBreakExecutionForNonRegisteredUser()
    {
        _context.Message = "/timeoff";

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);
        
        await this._command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Вы – незарегистрированный пользователь.\r\n\r\n" +
                                                          "Выполните команды:\r\n" +
                                                          "/registeruser\r\n" +
                                                          "/registermail"));
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
    }

    [Test]
    public async Task ShouldBreakExecutionForUserWithoutMail()
    {
        _context.Message = "/timeoff";

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { JobTitleGenitive = ""});
        
        await this._command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "У вас не зарегистрирована почта.\r\n" +
                                                          "Выполните команду: /registermail"));
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
    }

    [Test]
    public async Task ShouldBreakExecutionForUserWithoutInfo()
    {
        _context.Message = "/timeoff";

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { AccessToken = ""});
        
        await this._command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "У вас не заданы данные о пользователе.\r\n" +
                                                          "Выполните команду /registeruser"));
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
    }
    
    [Test]
    public async Task ShouldExecuteSkipCheckEmailsForRepeat()
    {
        _context.Message = "/timeoff";
        await this._command.Execute();
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
        
        _context.Message = "30.08.2022";
        await this._command.OnMessage();
        Assert.That(_command.Data.Period, Is.EqualTo("30.08.2022"));
        
        _context.Message = "Нужно помыть хомячка";
        await this._command.OnMessage();
        Assert.That(_command.Data.Reason, Is.EqualTo("Нужно помыть хомячка"));

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User());
        _context.Message = "Отработаю на следующей неделе";
        await this._command.OnMessage();
        Assert.That(_command.Data.WorkingOff, Is.EqualTo("Отработаю на следующей неделе"));
        _client.Verify(target => target.SendDocument(2517, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document(1, ""));
        _emailStorage
            .Setup(target => target.GetForDocument(It.IsAny<long>()))
            .ReturnsAsync(new [] { new Email("a.pushkin@infinnity.ru") });
        _context.Message = "Да";
        await this._command.OnMessage();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
        _context.Message = "Повторить";
        await this._command.OnMessage();
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
        _mailClient.Verify(target => target.SendMail(It.IsAny<SecretaryMailMessage>()), Times.Once);
    }
    
    [Test]
    public async Task ShouldExecuteShowCheckEmailsForNewEmail()
    {
        _context.Message = "/timeoff";
        await this._command.Execute();
        
        _context.Message = "30.08.2022";
        await this._command.OnMessage();
        Assert.That(_command.Data.Period, Is.EqualTo("30.08.2022"));
        
        _context.Message = "Нужно помыть хомячка";
        await this._command.OnMessage();
        Assert.That(_command.Data.Reason, Is.EqualTo("Нужно помыть хомячка"));

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User());
        _context.Message = "Отработаю на следующей неделе";
        await this._command.OnMessage();
        Assert.That(_command.Data.WorkingOff, Is.EqualTo("Отработаю на следующей неделе"));
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
        _context.Message = "/timeoff";
        await this._command.Execute();
        
        _context.Message = "30.08.2022";
        await this._command.OnMessage();
        Assert.That(_command.Data.Period, Is.EqualTo("30.08.2022"));
        
        _context.Message = "Нужно помыть хомячка";
        await this._command.OnMessage();
        Assert.That(_command.Data.Reason, Is.EqualTo("Нужно помыть хомячка"));

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User());
        _context.Message = "Отработаю на следующей неделе";
        await this._command.OnMessage();
        Assert.That(_command.Data.WorkingOff, Is.EqualTo("Отработаю на следующей неделе"));
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
        _context.Message = "/timeoff";
        await this._command.Execute();
        
        _context.Message = "30.08.2022";
        await this._command.OnMessage();
        Assert.That(_command.Data.Period, Is.EqualTo("30.08.2022"));
        
        _context.Message = "Нужно помыть хомячка";
        await this._command.OnMessage();
        Assert.That(_command.Data.Reason, Is.EqualTo("Нужно помыть хомячка"));

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User());
        _context.Message = "Отработаю на следующей неделе";
        await this._command.OnMessage();
        Assert.That(_command.Data.WorkingOff, Is.EqualTo("Отработаю на следующей неделе"));
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
}