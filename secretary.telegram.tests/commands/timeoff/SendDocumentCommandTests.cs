using MailKit.Security;
using Moq;
using secretary.documents.creators;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.timeoff;
using secretary.yandex.mail;

namespace secretary.telegram.tests.commands.subcommands.timeoff;

public class SendDocumentCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IMailClient> _mailClient = null!;
    
    private Mock<IDocumentStorage> _documentStorage = null!;
    private Mock<IEmailStorage> _emailStorage = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ITimeOffCreator> _timeOffCreator = null!;
    
    
    private TimeOffCommand _parent = null!;
    private SendDocumentCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        this._documentStorage = new Mock<IDocumentStorage>();
        this._emailStorage = new Mock<IEmailStorage>();
        this._userStorage = new Mock<IUserStorage>();
        this._client = new Mock<ITelegramClient>();
        this._mailClient = new Mock<IMailClient>();
        this._timeOffCreator = new Mock<ITimeOffCreator>();

        this._command = new SendDocumentCommand();

        this._command.MessageCreator = this._timeOffCreator.Object;
        
        this._parent = new TimeOffCommand();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            DocumentStorage = this._documentStorage.Object,
            EmailStorage = this._emailStorage.Object,
            UserStorage = this._userStorage.Object,
            MailClient = this._mailClient.Object,
        };
    }
    
    [Test]
    public async Task ShouldSkipRunCommandForNo()
    {
        _context.Message = "Нет";
        
        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document());

        await _command.Execute(_context);

        _documentStorage
            .Verify(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
    }
    
    [Test]
    public async Task ShouldAskEmails()
    {
        _context.Message = "Да";

        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document() { Id = 0 });

        _emailStorage
            .Setup(target => target.GetForDocument(It.IsAny<long>()))
            .ReturnsAsync(new Email[0]);

        _client.Setup(target => target.SendMessage(It.IsAny<long>(), It.IsAny<string>()));

        await _command.Execute(_context);

        _client.Verify(target => target.SendMessage(
            2517,
            "Отправьте список адресов для рассылки в формате:\r\n" +
            "<code>" +
            "a.pushkin@infinnity.ru (Александр Пушкин)\r\n" +
            "s.esenin@infinnity.ru (Сергей Есенин)\r\n" +
            "v.mayakovskii@infinnity.ru\r\n" +
            "</code>\r\n\r\n" +
            "Если вы укажете адрес без имени в скобках, то в имени отправителя будет продублированпочтовый адрес"
        ));
    }
    
    [Test]
    public async Task ShouldAskRepeat()
    {
        _context.Message = "Да";

        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document() { Id = 0 });

        _emailStorage
            .Setup(target => target.GetForDocument(It.IsAny<long>()))
            .ReturnsAsync(new []
            {
                new Email() { Address = "a.pushkin@infinnity.ru", DisplayName = "Александр Пушкин"},
                new Email() { Address = "s.esenin@infinnity.ru", DisplayName = "Сергей Есенин"},
                new Email() { Address = "v.mayakovskii@infinnity.ru"},
            });

        _client.Setup(target => target.SendMessage(It.IsAny<long>(), It.IsAny<string>()));

        await _command.Execute(_context);

        _client.Verify(target => target.SendMessageWithKeyBoard(
            2517,
            "В прошлый раз вы сделали рассылку на эти адреса:\r\n" +
            "<code>\r\n" +
            "a.pushkin@infinnity.ru (Александр Пушкин)\r\n" +
            "s.esenin@infinnity.ru (Сергей Есенин)\r\n" +
            "v.mayakovskii@infinnity.ru" +
            "</code>\r\n" +
            "\r\n" +
            "Повторить?",
            new []{ "Повторить" }
        ));
    }

    [Test]
    public async Task ShouldRepeatMessages()
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

        _context.Message = "Повторить";
        _parent.Data.FilePath = "timeoff.docx";
        _parent.Data.Period = "28.08.2022";

        await this._command.OnMessage(_context, _parent);

        var expectedReceivers = new[]
        {
            new SecretaryMailAddress("a.pushkin@infinnity.ru", "Александр Пушкин"),
            new SecretaryMailAddress("s.esenin@infinnity.ru", "Сергей Есенин"),
            new SecretaryMailAddress("v.mayakovskii@infinnity.ru", null),
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
                    It.Is<SecretaryMailMessage>(data => data.Theme == "[Отгул 28.08.2022]")
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
    }

    [Test]
    public async Task ShouldParseMails()
    {
        var user = new User()
        {
            Email = "user@infinnity.ru",
            Name = "Пользовалель Пользователев",
            JobTitle = "инженер-программист"
        };
        
        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document() { Id = 0 });

        _userStorage
            .Setup(target => target.GetUser(It.IsAny<long>()))
            .ReturnsAsync(user);
        
        _timeOffCreator.Setup(target => target.Create(It.IsAny<TimeOffData>())).Returns("html");

        _context.Message = "a.pushkin@infinnity.ru (Александр Пушкин)\n" +
                           "s.esenin@infinnity.ru (Сергей Есенин)\n" +
                           "v.mayakovskii@infinnity.ru";
        
        _parent.Data.FilePath = "timeoff.docx";
        _parent.Data.Period = "28.08.2022";

        await this._command.OnMessage(_context, _parent);

        var expectedReceivers = new[]
        {
            new SecretaryMailAddress("a.pushkin@infinnity.ru", "Александр Пушкин"),
            new SecretaryMailAddress("s.esenin@infinnity.ru", "Сергей Есенин"),
            new SecretaryMailAddress("v.mayakovskii@infinnity.ru", null!),
            new SecretaryMailAddress("user@infinnity.ru", "Пользовалель Пользователев"),
        };
        
        var expectedEmails = new[]
        {
            new Email() { Address = "a.pushkin@infinnity.ru", DisplayName = "Александр Пушкин" },
            new Email() { Address = "s.esenin@infinnity.ru", DisplayName = "Сергей Есенин" },
            new Email() { Address = "v.mayakovskii@infinnity.ru" },
        };
        
        _emailStorage.Verify(target => target.SaveForDocument(0, expectedEmails));

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
                    It.Is<SecretaryMailMessage>(data => data.Theme == "[Отгул 28.08.2022]")
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
    }
}