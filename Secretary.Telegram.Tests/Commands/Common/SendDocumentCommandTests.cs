using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Moq;
using Secretary.Cache;
using Secretary.Documents.utils;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Common;
using Secretary.Telegram.Models;
using Secretary.Telegram.Sessions;
using Secretary.Telegram.Utils;
using Secretary.Yandex.Mail;
using Secretary.Yandex.Mail.Data;

namespace Secretary.Telegram.Tests.Commands.Common;

public class SendDocumentCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IMailSender> _mailSender = null!;
    private Mock<ICacheService> _cacheService = null!;
    private Mock<TimeOffCache> _cache = null!;
    
    private Mock<IDocumentStorage> _documentStorage = null!;
    private Mock<IEmailStorage> _emailStorage = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<IFileManager> _fileManager = null!;
    
    
    private SendDocumentCommand<TimeOffCache> _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        _documentStorage = new Mock<IDocumentStorage>();
        _emailStorage = new Mock<IEmailStorage>();
        _cacheService = new Mock<ICacheService>();
        _cache = new Mock<TimeOffCache>();
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();
        _mailSender = new Mock<IMailSender>();
        _sessionStorage = new Mock<ISessionStorage>();
        _fileManager = new Mock<IFileManager>();
        

        _command = new SendDocumentCommand<TimeOffCache>();
        _command.FileManager = _fileManager.Object;
        
        _context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = _client.Object, 
            DocumentStorage = _documentStorage.Object,
            EmailStorage = _emailStorage.Object,
            UserStorage = _userStorage.Object,
            MailSender = _mailSender.Object,
            SessionStorage = _sessionStorage.Object,
            CacheService = _cacheService.Object,
        };
        
        _command.Context = _context;
    }

    [Test]
    public async Task ShouldSendMessage()
    {
        var emails = new[]
        {
            new Email() { Address = "a.pushkin@infinnity.ru", DisplayName = "Александр Пушкин" },
            new Email() { Address = "s.esenin@infinnity.ru", DisplayName = "Сергей Есенин" },
            new Email() { Address = "v.mayakovskii@infinnity.ru" },
        };

        var user = new User()
        {
            Email = "user@infinnity.ru",
            Name = "Пользовалель Пользователев",
            JobTitle = "инженер-программист"
        };
        
        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document() { Id = 0 });

        _emailStorage
            .Setup(target => target.GetForDocument(It.IsAny<long>()))
            .ReturnsAsync(emails);

        _userStorage
            .Setup(target => target.GetUser(It.IsAny<long>()))
            .ReturnsAsync(user);

        _cache.SetupGet(target => target.FilePath).Returns("timeoff.docx");
        _cache.SetupGet(target => target.DocumentKey).Returns("/timeoff");
        _cache.SetupGet(target => target.Period).Returns(new DatePeriodParser().Parse("28.08.2022"));
        _cache.Setup(target => target.CreateMail(It.IsAny<User>())).Returns("html");

        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(It.IsAny<long>()))
            .ReturnsAsync(_cache.Object);

        _context.Message = "Повторить";

        await _command.Execute();

        var expectedReceivers = new[]
        {
            new MailAddress("a.pushkin@infinnity.ru", "Александр Пушкин"),
            new MailAddress("s.esenin@infinnity.ru", "Сергей Есенин"),
            new MailAddress("v.mayakovskii@infinnity.ru", null!),
        };
        
        _mailSender
            .Verify(target => target.SendMail(
                    It.Is<MailMessage>(data => data.Receivers.SequenceEqual(expectedReceivers))
                )
            );
        
        _mailSender
            .Verify(target => target.SendMail(
                    It.Is<MailMessage>(data => data.Sender.Equals(new MailAddress("user@infinnity.ru", "Пользовалель Пользователев")))
                )
            );
        
        _mailSender
            .Verify(target => target.SendMail(
                    It.Is<MailMessage>(data => data.Theme == "Пользовалель Пользователев [Отгул 28.08.2022]")
                )
            );
        
        _mailSender
            .Verify(target => target.SendMail(
                    It.Is<MailMessage>(data => 
                        data.Attachments.Count() == 1 &&
                        data.Attachments.First().Path == "timeoff.docx" &&
                        data.Attachments.First().FileName == "Заявление на отгул.docx" &&
                        data.Attachments.First().ContentType.MediaType == "application" &&
                        data.Attachments.First().ContentType.MediaSubtype == "msword")
                )
            );
        
        _mailSender
            .Verify(target => target.SendMail(
                It.Is<MailMessage>(data => 
                    data.HtmlBody == "html"
                )
            ));
        
        _client.Verify(target => target.SendMessage(2517, "Заяление отправлено"));
        
        _cacheService.Verify(target => target.DeleteEntity<TimeOffCache>(2517));
        
        _fileManager.Verify(target => target.DeleteFile("timeoff.docx"));
    }

    [Test]
    public async Task ShouldProtectIncorrectRights()
    {
        _mailSender.Setup(target => target.SendMail(It.IsAny<MailMessage>()))
            .ThrowsAsync(new AuthenticationException("This user does not have access rights to this service"));

        _command.Context = _context;
        
        await _command.SendMail(null!);
        
        _client.Verify(target => target.SendMessage(
            2517, 
            "Не достаточно прав для отправки письма!\n\n" +
            "Убедитесь, что токен выдан для вашего рабочего почтового ящика.\n" +
            "Если ящик нужный, то перейдите в <a href=\"https://mail.yandex.ru/#setup/client\">настройки</a> " +
            "и разрешите отправку по OAuth-токену с сервера imap.\n" +
            "Не спешите пугаться незнакомых слов, вам просто нужно поставить одну галочку по ссылке"
        ));
        
        _cacheService.Verify(target => target.DeleteEntity<TimeOffCache>(2517));
    }

    [Test]
    public async Task ShouldProtectIncorrectToken()
    {
        _mailSender.Setup(target => target.SendMail(It.IsAny<MailMessage>()))
            .ThrowsAsync(new AuthenticationException("Invalid user or password"));

        _command.Context = _context;
        await _command.SendMail(null!);
        
        _client.Verify(target => target.SendMessage(
            2517, 
            "Невалидный токен!\n\n" +
            "Выполните команду /renewtoken"
        ));
        
        _userStorage.Verify(target => target.RemoveTokens(2517));
        
        _cacheService.Verify(target => target.DeleteEntity<TimeOffCache>(2517));
    }

    [Test]
    public async Task ShouldProtectIncorrectPermissionScope()
    {
        _mailSender.Setup(target => target.SendMail(It.IsAny<MailMessage>()))
            .ThrowsAsync(new AuthenticationException("Authentication failed"));

        _command.Context = _context;
        await _command.SendMail(null!);
        
        _client.Verify(target => target.SendMessage(
            2517, 
            "Невалидный токен!\n\n" +
            "Выполните команду /renewtoken"
        ));
        
        _userStorage.Verify(target => target.RemoveTokens(2517));
        
        _cacheService.Verify(target => target.DeleteEntity<TimeOffCache>(2517));
    }

    [Test]
    public async Task ShouldProtectNotOwnTokenUsage()
    {
        var exception = new SmtpCommandException(
            SmtpErrorCode.SenderNotAccepted, 
            SmtpStatusCode.AuthenticationRequired,
            new MailboxAddress("Username", "a.pushkin@infinnity.ru"),
            "Sender address rejected: not owned by auth user"
            );
        
        _mailSender.Setup(target => target.SendMail(It.IsAny<MailMessage>()))
            .ThrowsAsync(exception);

        _command.Context = _context;
        await _command.SendMail(new MailMessage()
        {
            Sender = new MailAddress("", "")
        });
        
        _client.Verify(target => target.SendSticker(
            2517, 
            Stickers.Guliki
        ));
        
        _client.Verify(target => target.SendMessage(
            2517, 
            "Вы отправляете письмо с токеном не принадлежащим ящику <code>a.pushkin@infinnity.ru</code>\n\n" +
            $"Выполните команду /renewtoken"
        ));
        
        _userStorage.Verify(target => target.RemoveTokens(2517));
        
        _cacheService.Verify(target => target.DeleteEntity<TimeOffCache>(2517));
    }

    [Test]
    public void ShouldGetNameWithPeriodTheme()
    {
        var user = new User()
        {
            Name = "Name",
            Email = "Email"
        };

        _cache.SetupGet(target => target.FilePath).Returns("FilePath");
        _cache.SetupGet(target => target.DocumentKey).Returns("/timeoff");
        _cache.SetupGet(target => target.Period)
            .Returns(new DatePeriod(new DateTime(2022, 09, 08), new DateTime(2022, 09, 12), ""));
        _cache.Setup(target => target.CreateMail(It.IsAny<User>())).Returns("");

        var result = _command.GetMailMessage(user, new Email[0], _cache.Object);
        
        Assert.That(result.Theme, Is.EqualTo("Name [Отгул 08.09.2022 — 12.09.2022]"));
    }
}