using Moq;
using Secretary.Cache;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.Documents.Common;
using Secretary.Telegram.Sessions;
using Secretary.Telegram.Utils;

namespace Secretary.Telegram.Tests.Commands.Documents.Common;

public class SetEmailsCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ICacheService> _cacheService = null!;
    private Mock<IDocumentStorage> _documentStorage = null!;
    private Mock<IEmailStorage> _emailStorage = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    
    private SetReceiversCommand<TimeOffCache> _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _cacheService = new Mock<ICacheService>();
        _documentStorage = new Mock<IDocumentStorage>();
        _emailStorage = new Mock<IEmailStorage>();
        _sessionStorage = new Mock<ISessionStorage>();

        _command = new SetReceiversCommand<TimeOffCache>();

        _context = new CommandContext()
        { 
            DocumentStorage = _documentStorage.Object,
            SessionStorage = _sessionStorage.Object,
            EmailStorage = _emailStorage.Object,
            CacheService = _cacheService.Object,
            TelegramClient = _client.Object, 
            UserMessage = new UserMessage { ChatId = 2517},
        };
        
        _command.Context = _context;
    }
    
    [Test]
    public async Task ShouldAskEmails()
    {
        _context.UserMessage.Text ="Да";

        var cacheMock = new Mock<TimeOffCache>();
        cacheMock.SetupGet(target => target.DocumentKey).Returns("/key");
        
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(It.IsAny<long>())).ReturnsAsync(cacheMock.Object);

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
            "Если вы укажете адрес без имени в скобках, то в имени отправителя будет продублирован почтовый адрес"
        ));
        
        _documentStorage.Verify(target => target.GetOrCreateDocument(2517, "/key"));
    }
    
    [Test]
    public async Task ShouldAskRepeat()
    {
        _context.UserMessage.Text ="Да";
        
        var cacheMock = new Mock<TimeOffCache>();
        cacheMock.SetupGet(target => target.DocumentKey).Returns("/key");
        
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(It.IsAny<long>())).ReturnsAsync(cacheMock.Object);

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

        _client.Verify(target => target.SendMessage(
            2517,
            "В прошлый раз вы сделали рассылку на эти адреса:\n" +
            "<code>\n" +
            "a.pushkin@infinnity.ru (Александр Пушкин)\n" +
            "s.esenin@infinnity.ru (Сергей Есенин)\n" +
            "v.mayakovskii@infinnity.ru" +
            "</code>\n" +
            "\n" +
            "Повторить?",
            TestUtils.ItIsReplayKeyBoard("Повторить" )
        ));
        
        _documentStorage.Verify(target => target.GetOrCreateDocument(2517, "/key"));
    }

    [Test]
    public async Task ShouldSkipNextOnRepeat()
    {
        _context.UserMessage.Text ="Повторить";

        var step = await _command.OnMessage();
        
        Assert.That(step, Is.EqualTo(2));
    }
    
    [Test]
    public async Task ShouldParseEmails()
    {
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(It.IsAny<long>()))
            .ReturnsAsync(new TimeOffCache() { Period = new DatePeriodParser().Parse("04.09.2022") });
        
        _context.UserMessage.Text ="a.pushkin@infinnity.ru (Александр Пушкин)\n" +
                           "s.esenin@infinnity.ru (Сергей Есенин)\n" +
                           "v.mayakovskii@infinnity.ru";
        
        var step = await _command.OnMessage();
        
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
        }, It.IsAny<int>()), Times.Once);
        
        Assert.That(step, Is.EqualTo(1));
    }
    
    [Test]
    public async Task ShouldReturnIncorrectEmailFormat()
    {
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(It.IsAny<long>()))
            .ReturnsAsync(new TimeOffCache() { Period = new DatePeriodParser().Parse("04.09.2022") });
        
        _context.UserMessage.Text ="a.pushkin@infinnity.ru (Александр Пушкин)\n" +
                           "s.esenin@infinnityru (Сергей Есенин)\n" +
                           "v.mayakovskii@infinnity.ru";
        
        var step = await _command.OnMessage();

        _client.Verify(target => target.SendMessage(2517, $"Почтовый адрес <code>s.esenin@infinnityru (Сергей Есенин)</code>" +
            " имеет некорректный формат.\n" +
            "Поправьте его и отправте список адресов еще раз."));
        
        Assert.That(step, Is.EqualTo(0));
    }
}