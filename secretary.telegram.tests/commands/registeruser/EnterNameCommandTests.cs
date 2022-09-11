using Moq;
using secretary.cache;
using secretary.telegram.commands;
using secretary.telegram.commands.caches;
using secretary.telegram.commands.registeruser;

namespace secretary.telegram.tests.commands.registeruser;

public class EnterNameCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private EnterNameCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._command = new EnterNameCommand();
        
        this._cacheService = new Mock<ICacheService>();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            CacheService = this._cacheService.Object,
        };
        
        this._command.Context = _context;
    }
    
    [Test]
    public async Task ShouldSendExampleMessage()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));

        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, "Введите ваши имя и фамилию в именительном падеже.\n" +
                                                              "Так они будут указаны в почтовом ящике, с которого будет отправляться письмо.\n" +
                                                              @"Например: <i>Александр Пушкин</i>"));
    }
    
    [Test]
    public async Task ShouldSetNameToCache()
    {
        _context.Message = "Александр Пушкин";
        
        await this._command.OnMessage();

        this._cacheService.Verify(target => target.SaveEntity<RegisterUserCache>(
                2517,
                new RegisterUserCache()
                {
                    Name = "Александр Пушкин"
                },
                It.IsAny<short>()
            ),
            Times.Once
        );
    }
}