using Moq;
using Secretary.Configuration;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Exceptions;

namespace Secretary.Telegram.Tests;

public class TelegramBotTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<IYandexAuthenticator> _yandexAuthenticator = null!;

    private Mock<Database> _database = null!;
    
    private TelegramBot _bot = null!;

    [SetUp]
    public void Setup()
    {
        _yandexAuthenticator = new Mock<IYandexAuthenticator>();
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();

        _database = new Mock<Database>();

        _database.SetupGet(target => target.UserStorage).Returns(_userStorage.Object);
        
        _bot = new TelegramBot(new Config(), _database.Object);
        _bot.TelegramClient = _client.Object;
    }

    [Test]
    public async Task ShouldHandleInvalidToken()
    {
        var user = new User()
        {
            ChatId = 2517,
            Email = "a.pushkin@infinnity.ru"
        };

        await _bot.HandleUserTokenExpired(user);
        
        _userStorage.Verify(target => target.RemoveTokens(2517));
        _client.Verify(target => target.SendMessage(2517, 
            "У вас истек токен для отправки почты!\n\n" +
                   $"Выполните команду /registermail для адреса a.pushkin@infinnity.ru"));
    }
}