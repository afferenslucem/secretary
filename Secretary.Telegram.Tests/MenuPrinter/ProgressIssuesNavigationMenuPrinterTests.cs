using Moq;
using Secretary.Telegram.Commands;
using Secretary.Telegram.MenuPrinters;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Tests.Commands.Menus;

public class ProgressIssuesNavigationMenuPrinterTests
{
    private Mock<ITelegramClient> _client = null!;
    private CommandContext _context = null!;
    private ProgressIssuesNavigationMenuPrinter _menuPrinter = null!;
    
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        
        _context = new CommandContext()
        {
            UserMessage = new UserMessage { ChatId = 2517, CallbackMessageId = 42},
            TelegramClient = _client.Object,
        };
    }
    
    [Test]
    public async Task ShouldPrintFirst2ButtonsSkip()
    {
        await new ProgressIssuesNavigationMenuPrinter(1, 5, 13).Print(_context);
        
        _client.Verify(
            target => target.SendMessage(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(
                    kb => kb.InlineKeyboard.ElementAt(0).ElementAt(0).Text == "🚫" &&
                          kb.InlineKeyboard.ElementAt(0).ElementAt(0).CallbackData == "/empty"
                )
            )
        );
        
        _client.Verify(
            target => target.SendMessage(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(
                    kb => kb.InlineKeyboard.ElementAt(0).ElementAt(1).Text == "🚫" &&
                          kb.InlineKeyboard.ElementAt(0).ElementAt(1).CallbackData == "/empty"
                )
            )
        );
    }
    
    [Test]
    public async Task ShouldPrintLast2ButtonsActive()
    {
        await new ProgressIssuesNavigationMenuPrinter(1, 5, 13).Print(_context);
        
        _client.Verify(
            target => target.SendMessage(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(
                    kb => kb.InlineKeyboard.ElementAt(0).ElementAt(2).Text == "▶" &&
                          kb.InlineKeyboard.ElementAt(0).ElementAt(2).CallbackData == "/progressissues 2"
                )
            )
        );
        
        _client.Verify(
            target => target.SendMessage(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(
                    kb => kb.InlineKeyboard.ElementAt(0).ElementAt(3).Text == "⏩" &&
                          kb.InlineKeyboard.ElementAt(0).ElementAt(3).CallbackData == "/progressissues 3"
                )
            )
        );
    }
    
    [Test]
    public async Task ShouldPrintLast2ButtonsSkip()
    {
        await new ProgressIssuesNavigationMenuPrinter(3, 5, 13).Print(_context);
        
        _client.Verify(
            target => target.SendMessage(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(
                    kb => kb.InlineKeyboard.ElementAt(0).ElementAt(2).Text == "🚫" &&
                          kb.InlineKeyboard.ElementAt(0).ElementAt(2).CallbackData == "/empty"
                )
            )
        );
        
        _client.Verify(
            target => target.SendMessage(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(
                    kb => kb.InlineKeyboard.ElementAt(0).ElementAt(3).Text == "🚫" &&
                          kb.InlineKeyboard.ElementAt(0).ElementAt(3).CallbackData == "/empty"
                )
            )
        );
    }
    
    [Test]
    public async Task ShouldPrintFirst2ButtonsActive()
    {
        await new ProgressIssuesNavigationMenuPrinter(3, 5, 13).Print(_context);
        
        _client.Verify(
            target => target.SendMessage(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(
                    kb => kb.InlineKeyboard.ElementAt(0).ElementAt(0).Text == "⏪" &&
                          kb.InlineKeyboard.ElementAt(0).ElementAt(0).CallbackData == "/progressissues 1"
                )
            )
        );
        
        _client.Verify(
            target => target.SendMessage(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(
                    kb => kb.InlineKeyboard.ElementAt(0).ElementAt(1).Text == "◀" &&
                          kb.InlineKeyboard.ElementAt(0).ElementAt(1).CallbackData == "/progressissues 2"
                )
            )
        );
    }
    
    [Test]
    public async Task ShouldPrintNextAndLastButtonWithDistance()
    {
        await new ProgressIssuesNavigationMenuPrinter(2, 5, 26).Print(_context);
        
        _client.Verify(
            target => target.SendMessage(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(
                    kb => kb.InlineKeyboard.ElementAt(0).ElementAt(2).Text == "▶" &&
                          kb.InlineKeyboard.ElementAt(0).ElementAt(2).CallbackData == "/progressissues 3"
                )
            )
        );
        
        _client.Verify(
            target => target.SendMessage(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(
                    kb => kb.InlineKeyboard.ElementAt(0).ElementAt(3).Text == "⏩" &&
                          kb.InlineKeyboard.ElementAt(0).ElementAt(3).CallbackData == "/progressissues 6"
                )
            )
        );
    }
    
    [Test]
    public async Task ShouldPrintFirstAndPrevButtonWithDistance()
    {
        await new ProgressIssuesNavigationMenuPrinter(5, 5, 26).Print(_context);
        
        _client.Verify(
            target => target.SendMessage(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(
                    kb => kb.InlineKeyboard.ElementAt(0).ElementAt(0).Text == "⏪" &&
                          kb.InlineKeyboard.ElementAt(0).ElementAt(0).CallbackData == "/progressissues 1"
                )
            )
        );
        
        _client.Verify(
            target => target.SendMessage(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(
                    kb => kb.InlineKeyboard.ElementAt(0).ElementAt(1).Text == "◀" &&
                          kb.InlineKeyboard.ElementAt(0).ElementAt(1).CallbackData == "/progressissues 4"
                )
            )
        );
    }
    
    [Test]
    public async Task ShouldPrintText()
    {
        await new ProgressIssuesNavigationMenuPrinter(5, 5, 26).Print(_context);
        
        _client.Verify(
            target => target.SendMessage(
                It.IsAny<long>(),
                "Страница: 5\n" +
                "Всего страниц: 6\n" +
                "Всего задач: 26",
                It.IsAny<InlineKeyboardMarkup>()
            )
        );
    }
}