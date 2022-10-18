using Moq;
using Secretary.Cache;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.Documents.SetEmails;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Sessions;

namespace Secretary.Telegram.Tests.Commands.Documents.SetEmails;

public class SetEmailsCommandTests
{
    private SetEmailsCommand _command = null!;
    private CommandContext _context = null!;
    
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ITelegramClient> _client = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<ICacheService> _cacheService = null!;
    private Mock<IEmailStorage> _emailService = null!;
    private Mock<IDocumentStorage> _documentService = null!;

    [SetUp]
    public void Setup()
    {
        _userStorage = new Mock<IUserStorage>();
        _client = new Mock<ITelegramClient>();
        _sessionStorage = new Mock<ISessionStorage>();
        _cacheService = new Mock<ICacheService>();
        _emailService = new Mock<IEmailStorage>();
        _documentService = new Mock<IDocumentStorage>();
        
        _command = new SetEmailsCommand();
        _context = new CommandContext()
        { 
            UserMessage = new UserMessage { ChatId = 2517},
            UserStorage = _userStorage.Object,
            TelegramClient = _client.Object,
            SessionStorage = _sessionStorage.Object,
            CacheService = _cacheService.Object,
            EmailStorage = _emailService.Object,
            DocumentStorage = _documentService.Object,
        };
        
        _command.Context = _context;
        
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(
            new User() { 
                Name = "Александр Пушкин",
                Email = "a.pushkin@infinnity.ru"
            }
        );
    }
    
    [Test]
    public void ShouldCreate()
    {
        Assert.Pass();
    }

    [Test]
    public void ShouldHaveCorrectKey()
    {
        Assert.That(SetEmailsCommand.Key, Is.EqualTo("/setemails"));
    }

    [Test]
    public async Task ShouldRunFully()
    {
        _context.UserMessage.Text ="/timeoff";
        
        await _command.Execute();

        _context.UserMessage.Text ="Отгул";

        _cacheService.Setup(target => target.GetEntity<SetEmailsCache>(2517)).ReturnsAsync(new SetEmailsCache()
        {
            DocumentKey = "/timeoff"
        });

        _documentService.Setup(target => target.GetOrCreateDocument(2517, "/timeoff"))
            .ReturnsAsync(new Document(2517, "/vacation") { Id = 1 } );
        
        _emailService.Setup(target => target.GetForDocument(1))
            .ReturnsAsync(new Email[0]);

        await _command.OnMessage();
        
        _context.UserMessage.Text ="a.pushkin@infinnity.ru";
        
        await _command.OnMessage();

        _context.UserMessage.Text ="Верно";
        
        await _command.OnMessage();
        
        _client.Verify(target => target.SendMessage(2517, "Адресаты успешно изменены"));
    }
    
    [Test]
    public void ShouldBreakExecutionForNonRegisteredUser()
    {
        _context.UserMessage.Text ="/timeoff";

        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);
        
        Assert.ThrowsAsync<NonCompleteUserException>(() => _command.Execute());
    }
}