using Moq;
using secretary.cache;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.caches;
using secretary.telegram.commands.timeoff;
using secretary.telegram.exceptions;
using secretary.telegram.sessions;
using secretary.telegram.utils;

namespace secretary.telegram.tests.commands.subcommands.timeoff;

public class SetEmailsCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ICacheService> _cacheService = null!;
    private Mock<IDocumentStorage> _documentStorage = null!;
    private Mock<IEmailStorage> _emailStorage = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    
    private TimeOffCommand _parent = null!;
    private SetEmailsCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();
        this._cacheService = new Mock<ICacheService>();
        this._documentStorage = new Mock<IDocumentStorage>();
        this._emailStorage = new Mock<IEmailStorage>();
        this._sessionStorage = new Mock<ISessionStorage>();

        this._command = new SetEmailsCommand();

        this._parent = new TimeOffCommand();

        this._context = new CommandContext()
        { 
            DocumentStorage = this._documentStorage.Object,
            SessionStorage = this._sessionStorage.Object,
            EmailStorage = this._emailStorage.Object,
            CacheService = this._cacheService.Object,
            TelegramClient = this._client.Object, 
            ChatId = 2517, 
        };
        
        this._command.Context = _context;
    }
    
    
    [Test]
    public void ShouldSkipRunCommandForNo()
    {
        _context.Message = "Нет";

        Assert.ThrowsAsync<ForceCompleteCommandException>(() =>_command.Execute());
        
        _client.Verify(target => target.SendMessage(2517, "Дальнейшее выполнение команды прервано"));
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
            "Отправьте список адресов для рассылки в формате:\n" +
            "<code>" +
            "a.pushkin@infinnity.ru (Александр Пушкин)\n" +
            "s.esenin@infinnity.ru (Сергей Есенин)\n" +
            "v.mayakovskii@infinnity.ru\n" +
            "</code>\n\n" +
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
            "В прошлый раз вы сделали рассылку на эти адреса:\n" +
            "<code>\n" +
            "a.pushkin@infinnity.ru (Александр Пушкин)\n" +
            "s.esenin@infinnity.ru (Сергей Есенин)\n" +
            "v.mayakovskii@infinnity.ru" +
            "</code>\n" +
            "\n" +
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
    public async Task ShouldParseEmails()
    {
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(It.IsAny<long>()))
            .ReturnsAsync(new TimeOffCache() { Period = new DatePeriodParser().Parse("04.09.2022") });
        
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
        
        _cacheService.Verify(target => target.SaveEntity(2517, new TimeOffCache()
        {
            Period = new DatePeriodParser().Parse("04.09.2022"),
            Emails = expectedEmails,
        }, It.IsAny<short>()), Times.Once);
        
        Assert.That(step, Is.EqualTo(1));
    }
    
    [Test]
    public async Task ShouldReturnIncorrectEmailFormat()
    {
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(It.IsAny<long>()))
            .ReturnsAsync(new TimeOffCache() { Period = new DatePeriodParser().Parse("04.09.2022") });
        
        _context.Message = "a.pushkin@infinnity.ru (Александр Пушкин)\n" +
                           "s.esenin@infinnityru (Сергей Есенин)\n" +
                           "v.mayakovskii@infinnity.ru";
        
        var step = await this._command.OnMessage();

        _client.Verify(target => target.SendMessage(2517, $"Почтовый адрес <code>s.esenin@infinnityru (Сергей Есенин)</code>" +
            " имеет некорректный формат.\n" +
            "Поправьте его и отправте список адресов еще раз."));
        
        Assert.That(step, Is.EqualTo(0));
    }
}