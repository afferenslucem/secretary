using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Moq;
using secretary.cache;
using secretary.documents.creators;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.caches;
using secretary.telegram.commands.timeoff;
using secretary.telegram.sessions;
using secretary.telegram.utils;
using secretary.yandex.mail;

namespace secretary.telegram.tests.commands.timeoff;

public class SendDocumentCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IMailClient> _mailClient = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private Mock<IDocumentStorage> _documentStorage = null!;
    private Mock<IEmailStorage> _emailStorage = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ITimeOffCreator> _timeOffCreator = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    
    
    private SendDocumentCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        this._documentStorage = new Mock<IDocumentStorage>();
        this._emailStorage = new Mock<IEmailStorage>();
        this._cacheService = new Mock<ICacheService>();
        this._userStorage = new Mock<IUserStorage>();
        this._client = new Mock<ITelegramClient>();
        this._mailClient = new Mock<IMailClient>();
        this._timeOffCreator = new Mock<ITimeOffCreator>();
        this._sessionStorage = new Mock<ISessionStorage>();

        this._command = new SendDocumentCommand();

        this._command.MessageCreator = this._timeOffCreator.Object;
        
        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            DocumentStorage = this._documentStorage.Object,
            EmailStorage = this._emailStorage.Object,
            UserStorage = this._userStorage.Object,
            MailClient = this._mailClient.Object,
            SessionStorage = this._sessionStorage.Object,
            CacheService = this._cacheService.Object,
        };
        
        this._command.Context = _context;
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
        
        _timeOffCreator.Setup(target => target.Create(It.IsAny<TimeOffData>())).Returns("html");

        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(It.IsAny<long>())).ReturnsAsync(new TimeOffCache()
        {
            Period = new DatePeriodParser().Parse("28.08.2022"),
            FilePath = "timeoff.docx"
        });

        _context.Message = "Повторить";

        await this._command.Execute();

        var expectedReceivers = new[]
        {
            new SecretaryMailAddress("a.pushkin@infinnity.ru", "Александр Пушкин"),
            new SecretaryMailAddress("s.esenin@infinnity.ru", "Сергей Есенин"),
            new SecretaryMailAddress("v.mayakovskii@infinnity.ru", null!),
            new SecretaryMailAddress("user@infinnity.ru", "Пользовалель Пользователев"),
        };
        
        _timeOffCreator.Verify(target => target.Create(
                It.Is<TimeOffData>(
                    data => data.Name == "Пользовалель Пользователев" &&
                            data.JobTitle == "инженер-программист" && 
                            data.Period == "28.08.2022"
                )
            )
        );
        
        _mailClient
            .Verify(target => target.SendMail(
                    It.Is<SecretaryMailMessage>(data => data.Receivers.SequenceEqual(expectedReceivers))
                )
            );
        
        _mailClient
            .Verify(target => target.SendMail(
                    It.Is<SecretaryMailMessage>(data => data.Sender.Equals(new SecretaryMailAddress("user@infinnity.ru", "Пользовалель Пользователев")))
                )
            );
        
        _mailClient
            .Verify(target => target.SendMail(
                    It.Is<SecretaryMailMessage>(data => data.Theme == "Пользовалель Пользователев [Отгул 28.08.2022]")
                )
            );
        
        _mailClient
            .Verify(target => target.SendMail(
                    It.Is<SecretaryMailMessage>(data => 
                        data.Attachments.Count() == 1 &&
                        data.Attachments.First().Path == "timeoff.docx" &&
                        data.Attachments.First().FileName == "Заявление.docx" &&
                        data.Attachments.First().ContentType.MediaType == "application" &&
                        data.Attachments.First().ContentType.MediaSubtype == "vnd.openxmlformats-officedocument.wordprocessingml.document")
                )
            );
        
        _mailClient
            .Verify(target => target.SendMail(
                It.Is<SecretaryMailMessage>(data => 
                    data.HtmlBody == "html"
                )
            ));
        
        _client.Verify(target => target.SendMessage(2517, "Заяление отправлено"));
        
        _cacheService.Verify(target => target.DeleteEntity<TimeOffCache>(2517));
    }

    [Test]
    public async Task ShouldProtectIncorrectRights()
    {
        _mailClient.Setup(target => target.SendMail(It.IsAny<SecretaryMailMessage>()))
            .ThrowsAsync(new AuthenticationException("This user does not have access rights to this service"));

        _command.Context = _context;
        await _command.SendMail(null!);
        
        _client.Verify(target => target.SendMessage(
            2517, 
            "Не достаточно прав для отправки письма!\r\n\r\n" +
            "Убедитесь, что токен выдан для вашего рабочего почтового ящика.\r\n" +
            "Если ящик нужный, то перейдите в <a href=\"https://mail.yandex.ru/#setup/client\">настройки</a> " +
            "и разрешите отправку по OAuth-токену с сервера imap.\r\n" +
            "Не спешите пугаться незнакомых слов, вам просто нужно поставить одну галочку по ссылке"
        ));
        
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
        
        _mailClient.Setup(target => target.SendMail(It.IsAny<SecretaryMailMessage>()))
            .ThrowsAsync(exception);

        _command.Context = _context;
        await _command.SendMail(null!);
        
        _client.Verify(target => target.SendSticker(
            2517, 
            Stickers.Guliki
        ));
        
        _client.Verify(target => target.SendMessage(
            2517, 
            "Вы отправляете письмо с токеном не принадлежащим ящику <code>a.pushkin@infinnity.ru</code>"
        ));
        
        _cacheService.Verify(target => target.DeleteEntity<TimeOffCache>(2517));
    }
}