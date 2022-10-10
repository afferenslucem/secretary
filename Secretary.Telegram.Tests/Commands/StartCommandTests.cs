using Moq;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Sessions;

namespace Secretary.Telegram.Tests.Commands;

public class StartCommandTests
{
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<ITelegramClient> _client = null!;

    private StartCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();

        _command = new StartCommand();
        _sessionStorage = new Mock<ISessionStorage>();

        _context = new CommandContext()
        {
            ChatId = 2517,
            TelegramClient = _client.Object,
            SessionStorage = _sessionStorage.Object,
        };

        _command.Context = _context;
    }

    [Test]
    public void ShouldHaveCorrectKey()
    {
        Assert.That(StartCommand.Key, Is.EqualTo("/start"));
    }

    [Test]
    public async Task ShouldSendExampleMessage()
    {
        await _command.Execute();

        _client.Verify(target => target.SendMessage(2517, "Добро пожаловать!\n" +
                                                               "\n" +
                                                               "Перед началом работы вам необходимо:\n" +
                                                               "1. /registeruser – зарегистрироваться\n" +
                                                               "2./registermail – зарегистрировать рабочую почту\n" +
                                                               "3. <a href=\"https://mail.yandex.ru/#setup/client\">Разрешить доступ по протоколу IMAP</a>"
                                                               )
        );
    }

    [Test]
    public async Task ShouldDeleteSession()
    {
        await _command.OnComplete();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
    }
}