using Moq;
using secretary.cache;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.caches;
using secretary.telegram.commands.registeruser;

namespace secretary.telegram.tests.commands.registeruser;

public class EnterJobTitleGenitiveCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private EnterJobTitleGenitiveCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._command = new EnterJobTitleGenitiveCommand();
        
        this._userStorage = new Mock<IUserStorage>();
        this._cacheService = new Mock<ICacheService>();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = _client.Object, 
            UserStorage = _userStorage.Object,
            CacheService = _cacheService.Object,
        };
        
        this._command.Context = _context;
    }
    
    [Test]
    public async Task ShouldSendJobTitleGenitiveCommand()
    {
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, "Введите вашу должность в родительном падеже.\n" +
                                                              "Так она будут указана в графе \"от кого\".\n" +
                                                              @"Например: От <i>поэта</i> Пушкина Александра Сергеевича"));
    }
    
    [Test]
    public async Task ShouldSetInsertUser()
    {
        var oldCache = new RegisterUserCache
        {
            Name = "Александр Пушкин",
            NameGenitive = "Пушкина Александра Сергеевича",
            JobTitle = "поэт"
        };
        
        _cacheService.Setup(obj => obj.GetEntity<RegisterUserCache>(It.IsAny<long>())).ReturnsAsync(oldCache);
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);

        _context.Message = "поэта";
        
        await this._command.OnMessage();
        
        this._userStorage.Verify(target => target.SetUser(
            It.Is<User>(user => user.ChatId == 2517 
                                && user.Name == "Александр Пушкин"
                                && user.NameGenitive == "Пушкина Александра Сергеевича" 
                                && user.JobTitle == "поэт" 
                                && user.JobTitleGenitive == "поэта" 
            )
        ));
        
        this._client.Verify(target => target.SendMessage(2517, "Ваш пользователь успешно сохранен"));
        
        this._cacheService.Verify(target => target.DeleteEntity<RegisterUserCache>(2517));
    }
    
    [Test]
    public async Task ShouldSetUpdateUser()
    {
        var oldCache = new RegisterUserCache
        {
            Name = "Александр Пушкин",
            NameGenitive = "Пушкина Александра Сергеевича",
            JobTitle = "поэт"
        };
        
        _cacheService.Setup(obj => obj.GetEntity<RegisterUserCache>(It.IsAny<long>())).ReturnsAsync(oldCache);

        var oldUser = new User()
        {
            Name = "Сергей Есенин",
            NameGenitive = "Есенина Сергея Александровича",
            JobTitle = "иманжинист",
            JobTitleGenitive = "иманжиниста",
        };
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);

        _context.Message = "поэта";
        
        await this._command.OnMessage();
        
        this._userStorage.Verify(target => target.SetUser(
            It.Is<User>(user => user.ChatId == 2517 
                                && user.Name == "Александр Пушкин"
                                && user.NameGenitive == "Пушкина Александра Сергеевича" 
                                && user.JobTitle == "поэт" 
                                && user.JobTitleGenitive == "поэта" 
            )
        ));
        
        this._client.Verify(target => target.SendMessage(2517, "Ваш пользователь успешно сохранен"));
        
        this._cacheService.Verify(target => target.DeleteEntity<RegisterUserCache>(2517));
    }
}