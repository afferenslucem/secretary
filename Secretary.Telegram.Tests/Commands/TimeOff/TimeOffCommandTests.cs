﻿using Moq;
using Secretary.Cache;
using Secretary.Configuration;
using Secretary.Documents;
using Secretary.Documents.utils;
using Secretary.Storage;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.TimeOff;
using Secretary.Telegram.Commands.Vacation;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Sessions;
using Secretary.Telegram.Utils;
using Secretary.Yandex.Mail;

namespace Secretary.Telegram.Tests.Commands.TimeOff;

public class TimeOffCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<IDocumentStorage> _documentStorage = null!;
    private Mock<IEmailStorage> _emailStorage = null!;
    private Mock<IMailClient> _mailClient = null!;
    private Mock<ICacheService> _cacheService = null!;
    private Mock<IFileManager> _fileManager = null!;

    private TimeOffCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _sessionStorage = new Mock<ISessionStorage>();
        _userStorage = new Mock<IUserStorage>();
        _documentStorage = new Mock<IDocumentStorage>();
        _emailStorage = new Mock<IEmailStorage>();
        _mailClient = new Mock<IMailClient>();
        _cacheService = new Mock<ICacheService>();
        _fileManager = new Mock<IFileManager>();

        _command = new TimeOffCommand();
        _command.FileManager = _fileManager.Object;
        
        _context = new CommandContext()
            { 
                ChatId = 2517, 
                TelegramClient = _client.Object, 
                SessionStorage = _sessionStorage.Object, 
                UserStorage = _userStorage.Object,
                DocumentStorage = _documentStorage.Object,
                EmailStorage = _emailStorage.Object,
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
        Assert.That(TimeOffCommand.Key, Is.EqualTo("/timeoff"));
    }
    
    [Test]
    public async Task ShouldSaveSessionOnExecute()
    {
        _sessionStorage.Setup(obj => obj.SaveSession(It.IsAny<long>(), It.IsAny<Session>()));

        await this._command.Execute();
        
        this._sessionStorage.Verify(target => target.SaveSession(2517, It.Is<Session>(session => session.ChatId == 2517 && session.LastCommand == _command)));
    }

    [Test]
    public void ShouldBreakExecutionForNonRegisteredUser()
    {
        _context.Message = "/timeoff";

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
        _context.Message = "/timeoff";

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { JobTitleGenitive = ""});
        
        Assert.ThrowsAsync<NonCompleteUserException>(() => this._command.Execute());
        
        _client.Verify(target => target.SendMessage(2517, "У вас не зарегистрирована почта.\n" +
                                                          "Выполните команду: /registermail"));
    }

    [Test]
    public void ShouldBreakExecutionForUserWithoutInfo()
    {
        _context.Message = "/timeoff";

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { AccessToken = ""});
        
        Assert.ThrowsAsync<NonCompleteUserException>(() => this._command.Execute());
        
        _client.Verify(target => target.SendMessage(2517, 
            "У вас не заданы данные о пользователе.\n" +
            "Выполните команду /registeruser"));
    }
    
    [Test]
    public async Task ShouldExecuteSkipCheckEmailsForRepeat()
    {
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(It.IsAny<long>())).ReturnsAsync(new TimeOffCache()
        {
            Period = new DatePeriodParser().Parse("05.09.2022"),
            WorkingOff = "Отаботаю",
            Reason = "Ну надо",
        });

        
        _context.Message = "/timeoff";
        await this._command.Execute();
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Never);
        
        _context.Message = "30.08.2022";
        await this._command.OnMessage();
        
        _context.Message = "Нужно помыть хомячка";
        await this._command.OnMessage();

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User());
        _context.Message = "Отработаю на следующей неделе";
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
        await _command.OnComplete();
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
        _mailClient.Verify(target => target.SendMail(It.IsAny<SecretaryMailMessage>()), Times.Once);
    }
    
    [Test]
    public async Task ShouldExecuteShowCheckEmailsForNewEmail()
    {
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(It.IsAny<long>())).ReturnsAsync(new TimeOffCache()
        {
            Period = new DatePeriodParser().Parse("05.09.2022"),
            WorkingOff = "Отаботаю",
            Reason = "Ну надо",
        });

        _context.Message = "/timeoff";
        await this._command.Execute();
        
        _context.Message = "30.08.2022";
        await this._command.OnMessage();
        
        _context.Message = "Нужно помыть хомячка";
        await this._command.OnMessage();

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User());
        _context.Message = "Отработаю на следующей неделе";
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
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(It.IsAny<long>())).ReturnsAsync(new TimeOffCache()
        {
            Period = new DatePeriodParser().Parse("05.09.2022"),
            WorkingOff = "Отаботаю",
            Reason = "Ну надо",
        });

        
        _context.Message = "/timeoff";
        await this._command.Execute();
        
        _context.Message = "30.08.2022";
        await this._command.OnMessage();
        
        _context.Message = "Нужно помыть хомячка";
        await this._command.OnMessage();

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User());
        _context.Message = "Отработаю на следующей неделе";
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
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(It.IsAny<long>())).ReturnsAsync(new TimeOffCache()
        {
            Period = new DatePeriodParser().Parse("05.09.2022"),
            WorkingOff = "Отаботаю",
            Reason = "Ну надо",
        });

        
        _context.Message = "/timeoff";
        await this._command.Execute();
        
        _context.Message = "30.08.2022";
        await this._command.OnMessage();
        
        _context.Message = "Нужно помыть хомячка";
        await this._command.OnMessage();

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User());
        _context.Message = "Отработаю на следующей неделе";
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
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(2517)).ReturnsAsync(
            new TimeOffCache
            {
                FilePath = "/timeoff-path.docx"
            }
        );

        await _command.OnForceComplete();
        
        _fileManager.Verify(target => target.DeleteFile("/timeoff-path.docx"));
        _cacheService.Verify(target => target.DeleteEntity<TimeOffCache>(2517));
        _sessionStorage.Verify(target => target.DeleteSession(2517));
    }
}