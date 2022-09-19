using Moq;
using Secretary.Cache;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Common;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Utils;

namespace Secretary.Telegram.Tests.Commands.Common;

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
        this._userStorage = new Mock<IUserStorage>();
        this._cacheService = new Mock<ICacheService>();
        this._client = new Mock<ITelegramClient>();
        this._cache = new Mock<TimeOffCache>();

        this._command = new CheckDocumentCommand<TimeOffCache>();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = _client.Object, 
            UserStorage = _userStorage.Object,
            CacheService = _cacheService.Object
        };
        
        this._command.Context = _context;
    }
    
    [Test]
    public async Task ShouldSendCheckDocumentCommand()
    {
        _cache.Setup(target => target.CreateDocument(It.IsAny<User>())).Returns("timeoff-path.docx");
        _cache.SetupGet(target => target.DocumentKey).Returns("/timeoff");
        _cache.SetupGet(target => target.Period).Returns(new DatePeriodParser().Parse("08.12.2022 с 9:00 до 13:00"));
        
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(2517)).ReturnsAsync(_cache.Object);
        
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { Name = "Александр Пушкин" } );

        _context.Message = "Да";
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Проверьте документ"));
        _client.Verify(target => target.SendDocument(2517, "timeoff-path.docx", "Заявление на отгул.docx"));
        _client.Verify(target => target.SendMessageWithKeyBoard(2517, "Отправить заявление?", new [] {"Да", "Нет"}));
        _cacheService.Verify(
            target => target.SaveEntity(
                2517, 
                _cache.Object,
                It.IsAny<short>()
            )
        );
    }
    
    [Test]
    public void ShouldSkipRunCommandForNo()
    {
        _context.Message = "Нет";

        Assert.ThrowsAsync<ForceCompleteCommandException>(() =>_command.OnMessage());
        
        _client.Verify(target => target.SendMessage(2517, "Дальнейшее выполнение команды прервано"));
    }
}