using Moq;
using Secretary.Cache;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Distant;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Utils;

namespace Secretary.Telegram.Tests.Commands.Distant;

public class EnterDistantPeriodCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private EnterDistantPeriodCommand<DistantCache> _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();
        _cacheService = new Mock<ICacheService>();

        _command = new EnterDistantPeriodCommand<DistantCache>();

        _context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            UserStorage = _userStorage.Object,
            CacheService = _cacheService.Object,
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
    public async Task ShouldSendGreetingCommand()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, 
            "Вы выбрали документ \"Удаленная работа\""));
    }
    
    [Test]
    public async Task ShouldSendEnterPeriodCommand()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, "Введите период удаленной работы в одном из форматов:\n\n" +
                                                               "Если на один день:\n<strong>DD.MM.YYYY[ с HH:mm до HH:mm]</strong>\n" +
                                                               "Например: <i>26.04.2020 c 9:00 до 13:00</i>\n\n" +
                                                               "Или, если на несколько дней:\n<strong>с DD.MM.YYYY до DD.MM.YYYY</strong>\n" +
                                                               "Например: <i>с 26.04.2020 до 28.04.2022</i>\n\n" +
                                                               "Или, на неопределенный срок:\n<strong>с DD.MM.YYYY до DD.MM.YYYY</strong>\n" +
                                                               "Например: <i>с 26.04.2020 на неопределенный срок</i>\n\n" +
                                                               "В таком виде это будет вставлено в документ"));
    }
    
    [Test]
    public async Task ShouldSetPeriodToCache()
    {
        _context.Message = "16.08.2022 c 13:00 до 17:00";
      
        await this._command.OnMessage();
        
        _cacheService.Verify(target => target.SaveEntity(2517, new DistantCache() { Period =
            new DatePeriodParser().Parse("16.08.2022 c 13:00 до 17:00")}, It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public void ShouldThrowIncorrectMessageExceptionForIncorrectDate()
    {
        _context.Message = "16.18.2022";
      
        Assert.ThrowsAsync<IncorrectMessageException>(() => _command.OnMessage());
        
        _client.Verify(target => target.SendMessage(2517, "Неверный формат периода отгула!\n" +
                                                          "Попробуйте еще раз"));
    }
    
    [Test]
    public void ShouldThrowIncorrectMessageExceptionForNonDateMessage()
    {
        _context.Message = "hello";
      
        Assert.ThrowsAsync<IncorrectMessageException>(() => _command.OnMessage());
        
        _client.Verify(target => target.SendMessage(2517, "Неверный формат периода отгула!\n" +
                                                                                          "Попробуйте еще раз"));
    }
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredUser()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);

        Assert.ThrowsAsync<NonCompleteUserException>(() => _command.Execute());
    }
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredMail()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { JobTitleGenitive = "" });

        Assert.ThrowsAsync<NonCompleteUserException>(() => _command.Execute());
    }
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredPersonalInfo()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { AccessToken = "" });

        Assert.ThrowsAsync<NonCompleteUserException>(() => _command.Execute());
    }
}