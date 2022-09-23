using Moq;
using Secretary.Cache;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.RegisterUser;

namespace Secretary.Telegram.Tests.Commands.RegisterUser;

public class EnterJobTitleCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private EnterJobTitleCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();

        _command = new EnterJobTitleCommand();
        
        _cacheService = new Mock<ICacheService>();

        _context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = _client.Object, 
            CacheService = _cacheService.Object,
        };
        
        _command.Context = _context;
    }
    
    [Test]
    public async Task ShouldSendExampleMessage()
    {
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Введите вашу должность в именительном падеже.\n" +
                                                               "Так она будут указана в подписи письма.\n" +
                                                               @"Например: С уважением, <i>поэт</i> Александр Пушкин"));
    }
    
    [Test]
    public async Task ShouldSetJobTitle()
    {
        var oldCache = new RegisterUserCache
        {
            Name = "Александр Пушкин",
            NameGenitive = "Пушкина Александра Сергеевича",
        };
        
        _cacheService.Setup(obj => obj.GetEntity<RegisterUserCache>(It.IsAny<long>())).ReturnsAsync(oldCache);

        _context.Message = "поэт";
        
        await _command.OnMessage();
        
        _cacheService.Verify(target => target.SaveEntity(
                2517,
                new RegisterUserCache()
                {
                    Name = "Александр Пушкин",
                    NameGenitive = "Пушкина Александра Сергеевича",
                    JobTitle = "поэт"
                },
                It.IsAny<int>()
            ),
            Times.Once
        );
    }
}