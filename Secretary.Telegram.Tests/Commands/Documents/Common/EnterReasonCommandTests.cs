using Moq;
using Secretary.Cache;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.Documents.Common;
using Secretary.Telegram.Utils;

namespace Secretary.Telegram.Tests.Commands.Documents.Common;

public class EnterReasonCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private EnterReasonCommand<TimeOffCache> _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _cacheService = new Mock<ICacheService>();

        _command = new EnterReasonCommand<TimeOffCache>();

        _context = new CommandContext()
        { 
            UserMessage = new UserMessage { ChatId = 2517},
            TelegramClient = _client.Object, 
            CacheService = _cacheService.Object,
        };
        
        _command.Context = _context;
    }
    
    [Test]
    public async Task ShouldSendEnterReasonCommand()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, 
            "Введите причину, это опционально, если вы нажмете \"Пропустить\", то этой графы не будет в заявлении.\n" +
            "А если укажете, то это будет строка вида\n<code>Причина: {{причина}}</code>",
            TestUtils.ItIsReplayKeyBoard("Пропустить")));
    }
    
    [Test]
    public async Task ShouldSetReasonToCommand()
    {
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(2517)).ReturnsAsync(new TimeOffCache() { Period =
            new DatePeriodParser().Parse("05.09.2022") });
        
        _context.UserMessage.Text ="Поеду заниматься ремонтом";

        await _command.OnMessage();
        
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
        _context.UserMessage.Text ="Пропустить";
        
        await _command.OnMessage();
        
        _cacheService.Verify(target => target.SaveEntity(2517, It.IsAny<TimeOffCache>(), It.IsAny<short>()), Times.Never);
    }
}