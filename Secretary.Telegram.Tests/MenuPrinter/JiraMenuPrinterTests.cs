using Moq;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Menus;
using Secretary.Telegram.MenuPrinters;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Tests.Commands.Menus;

public class JiraMenuPrinterTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    
    private JiraMenuPrinter _menuPrinter = null!;
    private CommandContext _context = null!;
    
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();
        
        _context = new CommandContext()
        {
            UserMessage = new UserMessage { ChatId = 2517, CallbackMessageId = 42},
            TelegramClient = _client.Object,
            UserStorage = _userStorage.Object,
        };

        _menuPrinter = new JiraMenuPrinter();
    }
    
    [Test]
    public async Task ShouldPrintText()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { RemindLogTime = false});

        await _menuPrinter.Print(_context);
        
        _client.Verify(target => target.SendMessage(
            2517,
            "Меню команд для Jira",
            It.IsAny<InlineKeyboardMarkup>()
        ));
    }
    
    [Test]
    public async Task ShouldPrintTurnOn()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { RemindLogTime = false});

        await _menuPrinter.Print(_context);
        
        _client.Verify(target => target.SendMessage(
            2517,
            It.IsAny<string>(),
            It.Is<InlineKeyboardMarkup>(keyboard => 
                keyboard.InlineKeyboard.ElementAt(4).FirstOrDefault(button => button.Text == "Включить напоминания о логгировани") != null
            )
        ));
    }

    [Test]
    public async Task ShouldPrintTurnOff()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { RemindLogTime = true});

        await _menuPrinter.Print(_context);
        
        _client.Verify(target => target.SendMessage(
            2517,
            It.IsAny<string>(),
            It.Is<InlineKeyboardMarkup>(keyboard => 
                keyboard.InlineKeyboard.ElementAt(4).FirstOrDefault(button => button.Text == "Выключить напоминания о логгировани") != null
            )
        ));
    }
    
    [Test]
    public async Task ShouldReprintTurnOn()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { RemindLogTime = false});

        await _menuPrinter.Reprint(_context);
        
        _client.Verify(target => target.EditMessage(
            2517,
            42,
            It.IsAny<string>(),
            It.Is<InlineKeyboardMarkup>(keyboard => 
                keyboard.InlineKeyboard.ElementAt(4).FirstOrDefault(button => button.Text == "Включить напоминания о логгировани") != null
            ))
        );
    }

    [Test]
    public async Task ShouldReprintTurnOff()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { RemindLogTime = true});

        await _menuPrinter.Reprint(_context);

        _client.Verify(target => target.EditMessage(
            2517,
            42,
            It.IsAny<string>(),
            It.Is<InlineKeyboardMarkup>(keyboard =>
                keyboard.InlineKeyboard.ElementAt(4)
                    .FirstOrDefault(button => button.Text == "Выключить напоминания о логгировани") != null
            ))
        );
    }

    [Test]
    public async Task ShouldReprintTurnOffAtSpecifiedMessage()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { RemindLogTime = true});

        await _menuPrinter.Reprint(_context, 24);

        _client.Verify(target => target.EditMessage(
            2517,
            24,
            It.IsAny<string>(),
            It.Is<InlineKeyboardMarkup>(keyboard =>
                keyboard.InlineKeyboard.ElementAt(4)
                    .FirstOrDefault(button => button.Text == "Выключить напоминания о логгировани") != null
            ))
        );
    }
}