using Moq;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Sessions;

namespace Secretary.Telegram.Tests.Commands;

public class UptimeCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;

    private UptimeCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();

        _command = new UptimeCommand();
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
        Assert.That(UptimeCommand.Key, Is.EqualTo("/uptime"));
    }

    [Test]
    public async Task ShouldSendVersion()
    {
        await _command.Execute();

        var expected = TelegramBot.Uptime.ToString("yyyy-MM-dd HH:mm:ss zz");

        _client.Verify(target => target.SendMessage(2517, expected));
    }

    [Test]
    public async Task ShouldDeleteSession()
    {
        await _command.OnComplete();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
    }
}
