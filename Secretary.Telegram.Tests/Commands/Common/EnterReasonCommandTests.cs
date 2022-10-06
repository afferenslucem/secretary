using Moq;
using Secretary.Cache;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Common;
using Secretary.Telegram.Utils;

namespace Secretary.Telegram.Tests.Commands.Common;

public class EnterReasonCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private EnterReasonCommand<TimeOffCache> _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();
        this._cacheService = new Mock<ICacheService>();

        this._command = new EnterReasonCommand<TimeOffCache>();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = _client.Object, 
            CacheService = _cacheService.Object,
        };
        
        this._command.Context = _context;
    }
    
    [Test]
    public async Task ShouldSendEnterReasonCommand()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, 
            "Введите причину, это опционально, если вы нажмете \"Пропустить\", то этой графы не будет в заявлении.\n" +
            "А если укажете, то это будет строка вида\n<code>Причина: {{причина}}</code>",
            TestUtils.IsItSameKeyBoards("Пропустить")));
    }
    
    [Test]
    public async Task ShouldSetReasonToCommand()
    {
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(2517)).ReturnsAsync(new TimeOffCache() { Period =
            new DatePeriodParser().Parse("05.09.2022") });
        
        _context.Message = "Поеду заниматься ремонтом";

        await this._command.OnMessage();
        
        _cacheService.Verify(target => target.SaveEntity(2517, 
            new TimeOffCache()
            {
                Period = new DatePeriodParser().Parse("05.09.2022"), 
                Reason = "Поеду заниматься ремонтом"
            }, It.IsAny<int>()), Times.Once);
    }
    
    
    [Test]
    public async Task ShouldSkipReasonToCommand()
    {
        _context.Message = "Пропустить";
        
        await this._command.OnMessage();
        
        _cacheService.Verify(target => target.SaveEntity(2517, It.IsAny<TimeOffCache>(), It.IsAny<short>()), Times.Never);
    }
}