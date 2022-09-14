using Moq;
using Secretary.Cache;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Common;
using Secretary.Telegram.Utils;

namespace Secretary.Telegram.Tests.Commands.Common;

public class EnterWorkingOffCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private EnterWorkingOffCommand<TimeOffCache> _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();
        this._cacheService = new Mock<ICacheService>();

        this._command = new EnterWorkingOffCommand<TimeOffCache>();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            CacheService = _cacheService.Object,
        };
        
        this._command.Context = _context;
    }
    
    [Test]
    public async Task ShouldSendEnterWorkingOffCommand()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessageWithKeyBoard(2517, 
            "Введите данные об отработке в свободном формате.\n" +
            "Например: <i>Отгул обязуюсь отработать</i>\n" +
            "Или: Отгул <i>без отработки</i>\n\n" +
            "Если вы нажмете \"Пропустить\", то эти данные просто не будут указаны в документе.",
            new [] { "Пропустить" }));
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
        
        _context.Message = "Отгул обязуюсь отработать";
        
        await this._command.OnMessage();
        
        _cacheService.Verify(
            target => target.SaveEntity(
                2517, 
                new TimeOffCache()
                {
                    Period = new DatePeriodParser().Parse("05.09.2022"), 
                    Reason = "Поеду заниматься ремонтом",
                    WorkingOff = "Отгул обязуюсь отработать",
                }, 
                It.IsAny<short>()), 
            Times.Once
        );
    }
    
    [Test]
    public async Task ShouldSkipWorkingOffCommand()
    {
        _context.Message = "Пропустить";
        
        await this._command.OnMessage();
        
        _cacheService.Verify(target => target.SaveEntity(2517, It.IsAny<TimeOffCache>(), It.IsAny<short>()), Times.Never);
    }
}