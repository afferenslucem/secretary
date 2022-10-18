using Moq;
using Secretary.Cache;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.Documents.Common;
using Secretary.Telegram.Utils;

namespace Secretary.Telegram.Tests.Commands.Documents.Common;

public class EnterWorkingOffCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private EnterWorkingOffCommand<TimeOffCache> _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _cacheService = new Mock<ICacheService>();

        _command = new EnterWorkingOffCommand<TimeOffCache>();

        _context = new CommandContext()
        { 
            UserMessage = new UserMessage { ChatId = 2517},
            TelegramClient = _client.Object, 
            CacheService = _cacheService.Object,
        };
        
        _command.Context = _context;
    }
    
    [Test]
    public async Task ShouldSendEnterWorkingOffCommand()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, 
            "Введите данные об отработке в свободном формате.\n" +
            "Например: <i>Отгул обязуюсь отработать</i>\n" +
            "Или: Отгул <i>без отработки</i>\n\n" +
            "Если вы нажмете \"Пропустить\", то эти данные просто не будут указаны в документе.",
            TestUtils.ItIsReplayKeyBoard("Пропустить" )));
    }
    
    [Test]
    public async Task ShouldSetWorkingOffToCommand()
    {
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(2517)).ReturnsAsync(
            new TimeOffCache()
            {
                Period = new DatePeriodParser().Parse("05.09.2022"), 
                Reason = "Поеду заниматься ремонтом",
            }
        );
        
        _context.UserMessage.Text ="Отгул обязуюсь отработать";
        
        await _command.OnMessage();
        
        _cacheService.Verify(
            target => target.SaveEntity(
                2517, 
                new TimeOffCache()
                {
                    Period = new DatePeriodParser().Parse("05.09.2022"), 
                    Reason = "Поеду заниматься ремонтом",
                    WorkingOff = "Отгул обязуюсь отработать",
                }, 
                It.IsAny<int>()), 
            Times.Once
        );
    }
    
    [Test]
    public async Task ShouldSkipWorkingOffCommand()
    {
        _context.UserMessage.Text ="Пропустить";
        
        await _command.OnMessage();
        
        _cacheService.Verify(target => target.SaveEntity(2517, It.IsAny<TimeOffCache>(), It.IsAny<short>()), Times.Never);
    }
}