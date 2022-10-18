using Moq;
using Secretary.Cache;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.Documents.Common;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Utils;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Tests.Commands.Documents.Common;

public class CheckDocumentCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ICacheService> _cacheService = null!;
    private Mock<TimeOffCache> _cache = null!;
    private CheckDocumentCommand<TimeOffCache> _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        _userStorage = new Mock<IUserStorage>();
        _cacheService = new Mock<ICacheService>();
        _client = new Mock<ITelegramClient>();
        _cache = new Mock<TimeOffCache>();

        _command = new CheckDocumentCommand<TimeOffCache>();

        _context = new CommandContext()
        { 
            UserMessage = new UserMessage { ChatId = 2517},
            TelegramClient = _client.Object, 
            UserStorage = _userStorage.Object,
            CacheService = _cacheService.Object
        };
        
        _command.Context = _context;
    }
    
    [Test]
    public async Task ShouldSendCheckDocumentCommand()
    {
        _cache.Setup(target => target.CreateDocument(It.IsAny<User>())).Returns("timeoff-path.docx");
        _cache.SetupGet(target => target.DocumentKey).Returns("/timeoff");
        _cache.SetupGet(target => target.Period).Returns(new DatePeriodParser().Parse("08.12.2022 с 9:00 до 13:00"));
        
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(2517)).ReturnsAsync(_cache.Object);
        
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { Name = "Александр Пушкин" } );

        _context.UserMessage.Text ="Да";
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Проверьте документ"));
        _client.Verify(target => target.SendDocument(2517, "timeoff-path.docx", "Заявление на отгул.docx"));
        _client.Verify(target => target.SendMessage(
            2517, 
            "Отправить заявление?", 
            TestUtils.ItIsReplayKeyBoard((ReplyKeyboardMarkup) new [] { "Да", "Нет" } )));
        _cacheService.Verify(
            target => target.SaveEntity(
                2517, 
                _cache.Object,
                It.IsAny<int>()
            )
        );
    }
    
    [Test]
    public void ShouldSkipRunCommandForNo()
    {
        _context.UserMessage.Text ="Нет";

        Assert.ThrowsAsync<ForceCompleteCommandException>(() =>_command.OnMessage());
        
        _client.Verify(target => target.SendMessage(2517, "Дальнейшее выполнение команды прервано"));
    }
}