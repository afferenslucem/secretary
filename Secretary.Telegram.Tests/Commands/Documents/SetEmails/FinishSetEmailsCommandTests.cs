using Moq;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Documents.SetEmails;

namespace Secretary.Telegram.Tests.Commands.Documents.SetEmails;

public class FinishSetEmailsCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private FinishSetEmailsCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _command = new FinishSetEmailsCommand();

        _context = new CommandContext()
        { 
            UserMessage = new UserMessage { ChatId = 2517},
            TelegramClient = _client.Object
        };
        
        _command.Context = _context;
    }
    
    [Test]
    public void ShouldCreate()
    {
        Assert.Pass();
    }

    [Test]
    public async Task ShouldSendGoodbye()
    {
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Адресаты успешно изменены"));
    }
}