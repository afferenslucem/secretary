using Moq;
using secretary.cache;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.caches;
using secretary.telegram.commands.registeruser;

namespace secretary.telegram.tests.commands.regusteruser;

public class EnterNameGenitiveCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private EnterNameGenitiveCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._command = new EnterNameGenitiveCommand();
        
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
    public async Task ShouldSendExample()
    {
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, "Введите ваши ФИО в родительном падеже.\r\n" +
                                                               "Так они будут указаны в отправляемом документе в графе \"от кого\".\r\n" +
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

        _context.Message = "Пушкина Александра Сергеевича";
        
        await this._command.OnMessage();
        
        

        this._cacheService.Verify(target => target.SaveEntity(
                2517,
                new RegisterUserCache()
                {
                    Name = "Александр Пушкин",
                    NameGenitive = "Пушкина Александра Сергеевича"
                },
                It.IsAny<short>()
            ),
            Times.Once
        );
    }
}