using Moq;
using secretary.telegram.commands;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands;

public class StartCommandTests
{
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<ITelegramClient> _client = null!;

    private StartCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._command = new StartCommand();
        this._sessionStorage = new Mock<ISessionStorage>();

        this._context = new CommandContext()
        {
            ChatId = 2517,
            TelegramClient = this._client.Object,
            SessionStorage = this._sessionStorage.Object,
        };

        this._command.Context = _context;
    }

    [Test]
    public void ShouldHaveCorrectKey()
    {
        Assert.That(StartCommand.Key, Is.EqualTo("/start"));
    }

    [Test]
    public async Task ShouldSendExampleMessage()
    {
        await this._command.Execute();

        this._client.Verify(target => target.SendMessage(2517, "Добро пожаловать!\n" +
                                                               "\n" +
                                                               "Перед началом работы вам необходимо:\n" +
                                                               "/registeruser – зарегистрироваться\n" +
                                                               "/registermail – зарегистрировать рабочую почту"));
    }

    [Test]
    public async Task ShouldDeleteSession()
    {
        await _command.OnComplete();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
    }
}