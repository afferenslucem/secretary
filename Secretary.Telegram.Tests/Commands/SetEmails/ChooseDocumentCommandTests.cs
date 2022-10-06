using Moq;
using Secretary.Cache;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.SetEmails;
using Secretary.Telegram.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Tests.Commands.SetEmails;

public class ChooseDocumentCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private CommandContext _context = null!;
    private ChooseDocumentCommand _command = null!;

    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();
        _cacheService = new Mock<ICacheService>();

        _context = new CommandContext
        {
            ChatId = 2517,
            TelegramClient = _client.Object,
            UserStorage = _userStorage.Object,
            CacheService = _cacheService.Object,
        };

        _command = new ChooseDocumentCommand();

        _command.Context = _context;
        
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(
            new User() { 
                Name = "Александр Пушкин",
                Email = "a.pushkin@infinnity.ru"
            }
        );
    }

    [Test]
    public async Task ShouldSendGreeting()
    {
        await _command.Execute();

        var items = new[] { "Отгул", "Отпуск", "Удаленная работа" };

        var keyboard = items
            .Select(InlineKeyboardButton.WithCallbackData)
            .Select(item => new [] {item})
            .ToArray();
        
        _client.Verify(target => target.SendMessage(2517,
            "Выберете документ для установки получателей",
            TestUtils.ItIsInlineKeyBoard(keyboard)
        ));
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
    
    [Test]
    public void ShouldThrowIncorrectMessageException()
    {
        _context.Message = "Ghbdtn";
        Assert.ThrowsAsync<IncorrectMessageException>(() => _command.OnMessage());
    }
    
    [Test]
    public async Task ShouldSaveCache()
    {
        _context.Message = "Отгул";

        await _command.OnMessage();
        
        _client.Verify(target => target.SendMessage(2517, "Выбран документ \"Отгул\""));
        _cacheService.Verify(target => target.SaveEntity(2517,
            It.Is<SetEmailsCache>(data => data.DocumentKey == "/timeoff"), It.IsAny<int>())
        );
    }
}