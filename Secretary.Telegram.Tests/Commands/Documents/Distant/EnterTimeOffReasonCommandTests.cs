using Moq;
using Secretary.Cache;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.Documents.Distant;
using Secretary.Telegram.Commands.Documents.TimeOff;
using Secretary.Telegram.Utils;

namespace Secretary.Telegram.Tests.Commands.Documents.Distant;

public class EnterDistantReasonCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private EnterDistantReasonCommand<DistantCache> _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _cacheService = new Mock<ICacheService>();

        _command = new EnterDistantReasonCommand<DistantCache>();

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
            "Введите причину перехода на удаленную работу\n" +
            "В документ это будет вставлено как строка вида\n<code>Причина: {{причина}}</code>"));
    }
    
    [Test]
    public async Task ShouldSetReasonToCommand()
    {
        _cacheService.Setup(target => target.GetEntity<DistantCache>(2517)).ReturnsAsync(new DistantCache() { Period =
            new DatePeriodParser().Parse("05.09.2022") });
        
        _context.UserMessage.Text ="Поеду заниматься ремонтом";

        await _command.OnMessage();
        
        _cacheService.Verify(target => target.SaveEntity(2517, 
            new DistantCache()
            {
                Period = new DatePeriodParser().Parse("05.09.2022"), 
                Reason = "Поеду заниматься ремонтом"
            }, It.IsAny<int>()), Times.Once);
    }
}