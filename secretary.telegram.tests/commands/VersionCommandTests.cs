using Moq;
using secretary.telegram.commands;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands;

public class VersionCommandTests
{
    private Mock<ITelegramClient> _client = null!;

    private VersionCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._command = new VersionCommand();

        this._context = new CommandContext()
        {
            ChatId = 2517,
            TelegramClient = this._client.Object,
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

        this._client.Verify(target => target.SendMessage(2517, "v0.4.0"));
    }
}