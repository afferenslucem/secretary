using Moq;
using Secretary.Cache;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.RegisterUser;

namespace Secretary.Telegram.Tests.Commands.RegisterUser;

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
        _client = new Mock<ITelegramClient>();

        _command = new EnterJobTitleGenitiveCommand();
        
        _userStorage = new Mock<IUserStorage>();
        _cacheService = new Mock<ICacheService>();

        _context = new CommandContext()
        { 
            UserMessage = new UserMessage { ChatId = 2517},
            TelegramClient = _client.Object, 
            UserStorage = _userStorage.Object,
            CacheService = _cacheService.Object,
        };
        
        _command.Context = _context;
    }
    
    [Test]
    public async Task ShouldSendJobTitleGenitiveCommand()
    {
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Введите вашу должность в родительном падеже.\n" +
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

        _context.UserMessage.Text ="поэта";
        
        await _command.OnMessage();
        
        _userStorage.Verify(target => target.SetUser(
            It.Is<User>(user => user.ChatId == 2517 
                                && user.Name == "Александр Пушкин"
                                && user.NameGenitive == "Пушкина Александра Сергеевича" 
                                && user.JobTitle == "поэт" 
                                && user.JobTitleGenitive == "поэта" 
            )
        ));
        
        _client.Verify(target => target.SendMessage(2517, "Ваш пользователь успешно сохранен"));
        
        _cacheService.Verify(target => target.DeleteEntity<RegisterUserCache>(2517));
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

        _context.UserMessage.Text ="поэта";
        
        await _command.OnMessage();
        
        _userStorage.Verify(target => target.SetUser(
            It.Is<User>(user => user.ChatId == 2517 
                                && user.Name == "Александр Пушкин"
                                && user.NameGenitive == "Пушкина Александра Сергеевича" 
                                && user.JobTitle == "поэт" 
                                && user.JobTitleGenitive == "поэта" 
            )
        ));
        
        _client.Verify(target => target.SendMessage(2517, "Ваш пользователь успешно сохранен"));
        
        _cacheService.Verify(target => target.DeleteEntity<RegisterUserCache>(2517));
    }
}