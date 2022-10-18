using Moq;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Jira;
using Secretary.Telegram.Commands.Menus;
using Secretary.Telegram.MenuPrinters;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Tests.Commands.Jira;

public class JiraMenuCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;

    private JiraMenuCommand _command = null!;
    private CommandContext _context = null!;
    
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();
        
        _command = new JiraMenuCommand();

        _context = new CommandContext()
        {
            UserMessage = new UserMessage { ChatId = 2517 },
            TelegramClient = _client.Object,
            UserStorage = _userStorage.Object,
        };

        _command.Context = _context;
    }

    [Test]
    public void ShouldHaveCorrectKey()
    {
        Assert.That(JiraMenuCommand.Key, Is.EqualTo("/jira"));
    }

    [Test]
    public async Task ShouldSendMessage()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { JiraPersonalAccessToken = "token", RemindLogTime = false});

        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(
            2517,
            "Меню команд для Jira",
            It.IsAny<InlineKeyboardMarkup>()
        ));
    }

    [Test]
    public async Task ShouldSendTurnOnRemind()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { JiraPersonalAccessToken = "token", RemindLogTime = false});

        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(
            2517,
            It.IsAny<string>(),
            It.Is<InlineKeyboardMarkup>(keyboard => 
                keyboard.InlineKeyboard.ElementAt(4).FirstOrDefault(button => button.Text == "Включить напоминания о логгировани") != null
            )
        ));
    }

    [Test]
    public async Task ShouldSendTurnOffRemind()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { JiraPersonalAccessToken = "token", RemindLogTime = true});

        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(
            2517,
            It.IsAny<string>(),
            It.Is<InlineKeyboardMarkup>(keyboard => 
                keyboard.InlineKeyboard.ElementAt(4).FirstOrDefault(button => button.Text == "Выключить напоминания о логгировани") != null
            )
        ));
    }

    [Test]
    public async Task ShouldSendEmptyTokenMessage()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { });

        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(
            2517,
            "У вас отсутствует PAT для JIRA\nВыполните команду /registerjiratoken"
        ));
    }
}