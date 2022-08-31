using Moq;
using secretary.telegram.commands;

namespace secretary.telegram.tests.commands;

public class StartCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    
    private StartCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._command = new StartCommand();
        
        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
        };
    }
    
    [Test]
    public async Task ShouldSendExampleMessage()
    {
        await this._command.Execute(_context);
        
        this._client.Verify(target => target.SendMessage(2517, "Добро пожаловать!\r\n" +
                                                               "\r\n" +
                                                               "Перед началом работы вам необходимо:\r\n" +
                                                               "/registeruser – зарегистрироваться\r\n" +
                                                               "/registermail – зарегистрировать рабочую почту"));
    }
}