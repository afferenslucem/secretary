using Moq;
using Secretary.Cache;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.TimeOff;
using Secretary.Telegram.Commands.Vacation;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Utils;

namespace Secretary.Telegram.Tests.Commands.Vacation;

public class EnterVacationPeriodCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private EnterVacationPeriodCommand<TimeOffCache> _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();
        _cacheService = new Mock<ICacheService>();

        _command = new EnterVacationPeriodCommand<TimeOffCache>();

        _context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = _client.Object, 
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
    public async Task ShouldSendEnterPeriodCommand()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Введите период отпуска в формате:\n" +
                                                               "<strong> с DD.MM.YYYY по DD.MM.YYYY</strong>\n" +
                                                               "Например: <i>с 07.02.2022 по 13.02.2022</i>\n\n" +
                                                               "В таком виде это будет вставлено в документ"));
    }
    
    [Test]
    public async Task ShouldSetPeriodToCache()
    {
        _context.Message = "с 16.08.2022 до 29.08.2022";
      
        await _command.OnMessage();
        
        _cacheService.Verify(target => target.SaveEntity(2517, new TimeOffCache() { Period =
            new DatePeriodParser().Parse("с 16.08.2022 до 29.08.2022")}, It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public async Task ShouldThrowExceptionForOneDayPeriod()
    {
        _context.Message = "16.08.2022 c 13:00 до 17:00";
      
        Assert.ThrowsAsync<IncorrectMessageException>(() => _command.OnMessage());
        
        _client.Verify(target => target.SendMessage(2517, "Неверный формат периода отпуска!\n" +
                                                          "Попробуйте еще раз"));
    }
    
    [Test]
    public void ShouldThrowIncorrectMessageExceptionForIncorrectDate()
    {
        _context.Message = "16.18.2022";
      
        Assert.ThrowsAsync<IncorrectMessageException>(() => _command.OnMessage());
        
        _client.Verify(target => target.SendMessage(2517, "Неверный формат периода отпуска!\n" +
                                                          "Попробуйте еще раз"));
    }
    
    [Test]
    public void ShouldThrowIncorrectMessageExceptionForNonDateMessage()
    {
        _context.Message = "hello";
      
        Assert.ThrowsAsync<IncorrectMessageException>(() => _command.OnMessage());
        
        _client.Verify(target => target.SendMessage(2517, "Неверный формат периода отпуска!\n" +
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