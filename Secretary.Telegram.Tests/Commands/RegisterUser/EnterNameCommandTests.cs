using Moq;
using Secretary.Cache;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.RegisterUser;

namespace Secretary.Telegram.Tests.Commands.RegisterUser;

public class EnterNameCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private EnterNameCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();

        _command = new EnterNameCommand();
        
        _cacheService = new Mock<ICacheService>();

        _context = new CommandContext()
        { 
            UserMessage = new UserMessage { ChatId = 2517},
            TelegramClient = _client.Object, 
            CacheService = _cacheService.Object,
        };
        
        _command.Context = _context;
    }
    
    [Test]
    public async Task ShouldSendExampleMessage()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));

        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Введите ваши имя и фамилию в именительном падеже.\n" +
                                                              "Так они будут указаны в почтовом ящике, с которого будет отправляться письмо.\n" +
                                                              @"Например: <i>Александр Пушкин</i>"));
    }
    
    [Test]
    public async Task ShouldSetNameToCache()
    {
        _context.UserMessage.Text ="Александр Пушкин";
        
        await _command.OnMessage();

        _cacheService.Verify(target => target.SaveEntity<RegisterUserCache>(
                2517,
                new RegisterUserCache()
                {
                    Name = "Александр Пушкин"
                },
                It.IsAny<int>()
            ),
            Times.Once
        );
    }
}