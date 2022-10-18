using Moq;
using Secretary.Cache;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.RegisterMail;
using Secretary.Telegram.Exceptions;
using Secretary.Yandex.Authentication;

namespace Secretary.Telegram.Tests.Commands.RegisterMail;

public class EnterEmailCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ICacheService> _cacheService = null!;
    private Mock<IYandexAuthenticator> _yandexAuthenticator = null!;
    private CommandContext _context= null!;
    private EnterEmailCommand _command= null!;
        
    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();
        _yandexAuthenticator = new Mock<IYandexAuthenticator>();
        _cacheService = new Mock<ICacheService>();

        _context = new CommandContext()
        {
            UserMessage = new UserMessage { ChatId = 2517},
            TelegramClient = _client.Object, 
            UserStorage = _userStorage.Object,
            CacheService = _cacheService.Object,
            YandexAuthenticator = _yandexAuthenticator.Object
        };

        _command = new EnterEmailCommand();
        _command.Context = _context;
    }

    [Test]
    public void ShouldCreate()
    {
        Assert.Pass();
    }
    
    [Test]
    public async Task ShouldSendEnterEmail()
    {
        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, "Введите вашу почту, с которой вы отправляете заявления.\n" +
                                                              @"Например: <i>a.pushkin@infinnity.ru</i>"));
    }
    
    [Test]
    public async Task ShouldSetCacheDataEmail()
    {
        _context.UserMessage.Text ="a.pushkin@infinnity.ru";
        
        await _command.OnMessage();
        
        _cacheService.Verify(target => target.SaveEntity(2517,  new RegisterMailCache("a.pushkin@infinnity.ru"), It.IsAny<int>()));
    }
        
    [Test]
    public void ShouldThrowErrorForIncorrectEmail()
    {
        _context.UserMessage.Text ="a.pushkin@infinnity";

        _command.Context = _context;
        
        Assert.ThrowsAsync<IncorrectMessageException>(async () => await _command.ValidateMessage());
        _client.Verify(target => target.SendMessage(2517, "Некорректный формат почты. Введите почту еще раз"));
    }
        
    [Test]
    public void ShouldThrowErrorForIncorrectDomain()
    {
        _yandexAuthenticator
            .Setup(target => target.IsUserDomainAllowed(It.IsAny<string>()))
            .Returns(false);

        _context.UserMessage.Text ="a.pushkin@gmail.com";

        _command.Context = _context;
        
        Assert.ThrowsAsync<IncorrectMessageException>(async () => await _command.ValidateMessage());
        _client.Verify(target => target.SendMessage(2517, "Некорректный домен почты.\n" +
                                                          "Бот доступен только для сотрудников Infinnity Solutions.\n" +
                                                          "Введите вашу рабочую почту"));
    }
}