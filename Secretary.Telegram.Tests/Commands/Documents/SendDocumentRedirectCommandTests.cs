using Moq;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Documents;
using Secretary.Telegram.Commands.Documents.Distant;
using Secretary.Telegram.Commands.Documents.TimeOff;
using Secretary.Telegram.Commands.Documents.Vacation;
using Secretary.Telegram.Commands.Menus;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Tests.Commands.Documents;

public class SendDocumentRedirectCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private SendDocumentRedirectCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();

        _command = new SendDocumentRedirectCommand();

        _context = new CommandContext()
        {
            UserMessage = new UserMessage { ChatId = 2517 },
            TelegramClient = _client.Object,
        };

        _command.Context = _context;
    }

    [Test]
    public void ShouldHaveCorrectKey()
    {
        Assert.That(SendDocumentRedirectCommand.Key, Is.EqualTo("/senddocument"));
    }

    [Test]
    public async Task ShouldSendExampleMessage()
    {
        await _command.Execute();

        var expectedKeyBoard = new[]
        {
            new [] { InlineKeyboardButton.WithCallbackData("Отгул", TimeOffCommand.Key) },
            new [] { InlineKeyboardButton.WithCallbackData("Отпуск", VacationCommand.Key) },
            new [] { InlineKeyboardButton.WithCallbackData("Удаленная работа", DistantCommand.Key) },
        };
        
        _client.Verify(target => target.SendMessage(2517, 
                "Выберите документ для отправки",
                TestUtils.ItIsInlineKeyBoard(expectedKeyBoard)
            )
        );
    }
}