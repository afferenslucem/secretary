using Moq;
using secretary.telegram.commands;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands;

public class VersionCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;

    private VersionCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._command = new VersionCommand();
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
        Assert.That(VersionCommand.Key, Is.EqualTo("/version"));
    }

    [Test]
    public async Task ShouldSendVersion()
    {
        await this._command.Execute();

        this._client.Verify(target => target.SendMessage(2517, "v0.4.2"));
    }

    [Test]
    public async Task ShouldDeleteSession()
    {
        await _command.OnComplete();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
    }
}