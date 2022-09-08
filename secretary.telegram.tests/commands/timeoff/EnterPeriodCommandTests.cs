using Moq;
using secretary.cache;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.caches;
using secretary.telegram.commands.timeoff;
using secretary.telegram.exceptions;
using secretary.telegram.utils;

namespace secretary.telegram.tests.commands.subcommands.timeoff;

public class EnterPeriodCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private EnterPeriodCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();
        this._userStorage = new Mock<IUserStorage>();
        this._cacheService = new Mock<ICacheService>();

        this._command = new EnterPeriodCommand();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            UserStorage = _userStorage.Object,
            CacheService = _cacheService.Object,
        };
        
        this._command.Context = _context;

        this._userStorage.Setup(target => target.GetUser(It.IsAny<long>()))
            .ReturnsAsync(new User() { JobTitleGenitive = "", AccessToken = "" });
    }
    
    [Test]
    public async Task ShouldSendEnterPeriodCommand()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, "Введите период отгула в одном из форматов:\n\n" +
                                                               "Если отгул на один день:\n<strong>DD.MM.YYYY[ с HH:mm до HH:mm]</strong>\n" +
                                                               "Например: <i>26.04.2020 c 9:00 до 13:00</i>\n\n" +
                                                               "Или, если отгул на несколько дней:\n<strong>с [HH:mm ]DD.MM.YYYY до [HH:mm ]DD.MM.YYYY</strong>\n" +
                                                               "Например: <i>с 9:00 26.04.2020 до 13:00 28.04.2022</i>\n\n" +
                                                               "В таком виде это будет вставлено в документ"));
    }
    
    [Test]
    public async Task ShouldSetPeriodToCache()
    {
        _context.Message = "16.08.2022 c 13:00 до 17:00";
      
        await this._command.OnMessage();
        
        _cacheService.Verify(target => target.SaveEntity(2517, new TimeOffCache() { Period =
            new DatePeriodParser().Parse("16.08.2022 c 13:00 до 17:00")}, It.IsAny<short>()), Times.Once);
    }
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredUser()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);

        Assert.ThrowsAsync<NonCompleteUserException>(() => this._command.Execute());
    }
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredMail()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { JobTitleGenitive = "" });

        Assert.ThrowsAsync<NonCompleteUserException>(() => this._command.Execute());
    }
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredPersonalInfo()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { AccessToken = "" });

        Assert.ThrowsAsync<NonCompleteUserException>(() => this._command.Execute());
    }
}