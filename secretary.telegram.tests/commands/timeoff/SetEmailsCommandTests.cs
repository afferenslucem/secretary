using Moq;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.timeoff;

namespace secretary.telegram.tests.commands.subcommands.timeoff;

public class SetEmailsCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IDocumentStorage> _documentStorage = null!;
    private Mock<IEmailStorage> _emailStorage = null!;
    
    private TimeOffCommand _parent = null!;
    private SetEmailsCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();
        this._documentStorage = new Mock<IDocumentStorage>();
        this._emailStorage = new Mock<IEmailStorage>();

        this._command = new SetEmailsCommand();

        this._parent = new TimeOffCommand();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            DocumentStorage = this._documentStorage.Object,
            EmailStorage = this._emailStorage.Object,
        };
        
        this._command.Context = _context;
        this._command.ParentCommand = _parent;
    }
    
    
    [Test]
    public async Task ShouldSkipRunCommandForNo()
    {
        _context.Message = "Нет";
        
        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document());

        await _command.Execute();

        _documentStorage
            .Verify(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
    }
    
    
    [Test]
    public async Task ShouldRunCommandForBackwardRedirect()
    {
        _context.Message = "Нет";
        _context.BackwardRedirect = true;
        
        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document());

        await _command.Execute();

        _documentStorage
            .Verify(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()), Times.Once);
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

        await _command.Execute();

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

        await _command.Execute();

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
    public async Task ShouldSkipNextOnRepeat()
    {
        _context.Message = "Повторить";

        var step = await _command.OnMessage();
        
        Assert.That(step, Is.EqualTo(2));
    }
    
    [Test]
    public async Task ShouldParseMails()
    {
        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document() { Id = 0 });
        
        _context.Message = "a.pushkin@infinnity.ru (Александр Пушкин)\n" +
                           "s.esenin@infinnity.ru (Сергей Есенин)\n" +
                           "v.mayakovskii@infinnity.ru";
        
        var step = await this._command.OnMessage();
        
        var expectedEmails = new[]
        {
            new Email() { Address = "a.pushkin@infinnity.ru", DisplayName = "Александр Пушкин" },
            new Email() { Address = "s.esenin@infinnity.ru", DisplayName = "Сергей Есенин" },
            new Email() { Address = "v.mayakovskii@infinnity.ru" },
        };
        
        _emailStorage.Verify(target => target.SaveForDocument(0, expectedEmails), Times.Once);
        
        Assert.That(step, Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldCancelCommandForNo()
    {
        _context.Message = "Нет";

        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Дальнейшее выполнение команды прервано"));
    }
}