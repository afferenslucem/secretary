using Moq;
using secretary.cache;
using secretary.documents.creators;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.caches;
using secretary.telegram.commands.timeoff;
using secretary.telegram.utils;

namespace secretary.telegram.tests.commands.subcommands.timeoff;

public class CheckDocumentCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ITimeOffCreator> _creator = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ICacheService> _cacheService = null!;
    private CheckDocumentCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        this._userStorage = new Mock<IUserStorage>();
        this._cacheService = new Mock<ICacheService>();
        this._client = new Mock<ITelegramClient>();
        this._creator = new Mock<ITimeOffCreator>();

        this._command = new CheckDocumentCommand();
        this._command.Creator = _creator.Object;

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
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(2517)).ReturnsAsync(new TimeOffCache() { Period =
            new DatePeriodParser().Parse("08.12.2022 с 9:00 до 13:00")});
        
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { Name = "Александр Пушкин" } );
        _creator.Setup(target => target.Create(It.IsAny<TimeOffData>())).Returns("timeoff-path.docx");

        _context.Message = "Да";
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Проверьте документ"));
        _client.Verify(target => target.SendDocument(2517, "timeoff-path.docx", "Александр-Пушкин-08.12.2022-Отгул.docx"));
        _client.Verify(target => target.SendMessageWithKeyBoard(2517, "Отправить заявление?", new [] {"Да", "Нет"}));
        _cacheService.Verify(
            target => target.SaveEntity(
                2517, 
                new TimeOffCache()
                {
                    Period = new DatePeriodParser().Parse("08.12.2022 с 9:00 до 13:00"),
                    FilePath = "timeoff-path.docx"
                },
                It.IsAny<short>()
            )
        );
    }
    
    [Test]
    public async Task ShouldSendDocumentWithPeriodName()
    {
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(2517)).ReturnsAsync(new TimeOffCache() { Period =
            new DatePeriodParser().Parse("с 9:00 08.12.2022 до 13:00 09.12.2022")});
        
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { Name = "Александр Пушкин" } );
        _creator.Setup(target => target.Create(It.IsAny<TimeOffData>())).Returns("timeoff-path.docx");

        _context.Message = "Да";
        
        await _command.Execute();
        
        _client.Verify(target => target.SendDocument(2517, "timeoff-path.docx", "Александр-Пушкин-08.12.2022—09.12.2022-Отгул.docx"));
    }
}