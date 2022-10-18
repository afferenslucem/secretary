using Moq;
using Secretary.Cache;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.RegisterUser;

namespace Secretary.Telegram.Tests.Commands.RegisterUser;

public class EnterNameGenitiveCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private EnterNameGenitiveCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();

        _command = new EnterNameGenitiveCommand();
        
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
    public async Task ShouldSendExample()
    {
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Введите ваши ФИО в родительном падеже.\n" +
                                                               "Так они будут указаны в отправляемом документе в графе \"от кого\".\n" +
                                                               @"Например: От <i>Пушкина Александра Сергеевича</i>"));
    }
    
    [Test]
    public async Task ShouldSetNameGenitiveToCache()
    {
        var oldCache = new RegisterUserCache
        {
            Name = "Александр Пушкин",
        };
        
        _cacheService.Setup(obj => obj.GetEntity<RegisterUserCache>(It.IsAny<long>())).ReturnsAsync(oldCache);

        _context.UserMessage.Text ="Пушкина Александра Сергеевича";
        
        await _command.OnMessage();
        
        

        _cacheService.Verify(target => target.SaveEntity(
                2517,
                new RegisterUserCache()
                {
                    Name = "Александр Пушкин",
                    NameGenitive = "Пушкина Александра Сергеевича"
                },
                It.IsAny<int>()
            ),
            Times.Once
        );
    }
}