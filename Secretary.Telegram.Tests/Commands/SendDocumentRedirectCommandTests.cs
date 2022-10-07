using Moq;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Distant;
using Secretary.Telegram.Commands.TimeOff;
using Secretary.Telegram.Commands.Vacation;
using Secretary.Telegram.Sessions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Tests.Commands;

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
            ChatId = 2517,
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