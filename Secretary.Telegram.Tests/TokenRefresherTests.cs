using Moq;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Exceptions;

namespace Secretary.Telegram.Tests;

public class TokenRefresherTests
{
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<IYandexAuthenticator> _yandexAuthenticator = null!;
    private Mock<ITelegramClient> _telegramClient = null!;
    
    public TokenRefresher _refresher = null!;

    [SetUp]
    public void Setup()
    {
        _yandexAuthenticator = new Mock<IYandexAuthenticator>();
        _userStorage = new Mock<IUserStorage>();
        _telegramClient = new Mock<ITelegramClient>();
        
        _refresher = new TokenRefresher(
            _yandexAuthenticator.Object, 
            _userStorage.Object, 
            _telegramClient.Object
        );

        _refresher.ShouldSkipDelay = true;
    }

    [Test]
    public async Task ShouldUpdateToken()
    {
        var user = new User();

        _yandexAuthenticator
            .Setup(
                target => target.RefreshToken(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            ).ReturnsAsync(new TokenData()
            {
                access_token = "access_token",
                refresh_token = "refresh_token",
                expires_in = 100,
            });

        await _refresher.RefreshToken(user);
        
        _userStorage.Verify(
            target => target.UpdateUser(It.Is<User>(item => item.AccessToken == "access_token"))
        );
        
        _userStorage.Verify(
            target => target.UpdateUser(It.Is<User>(item => item.RefreshToken == "refresh_token"))
        );
        
        _userStorage.Verify(
            target => target.UpdateUser(It.Is<User>(item => item.TokenExpirationSeconds == 100))
        );
    }

    [Test]
    public async Task ShouldSkipUpdateTokensForUsersWithoutRefresh()
    {
        var user = new User();
        var user2 = new User()
        {
            RefreshToken = "refresh_token"
        };
        
        await _refresher.RefreshTokens(new [] { user, user2 });
        
        _yandexAuthenticator.Verify(
            target => target.RefreshToken(
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>()),
            Times.Exactly(1)
        );
        
        _yandexAuthenticator.Verify(
            target => target.RefreshToken(
                "refresh_token", 
                It.IsAny<CancellationToken>())
        );
    }

    [Test]
    public async Task ShouldRefreshForAllUsers()
    {
        _userStorage.Setup(target => target.GetCount()).ReturnsAsync(25);

        await _refresher.RefreshTokensForAllUsers();
        
        _userStorage.Verify(
            target => target.GetUsers(It.IsAny<int>(), It.IsAny<int>()), 
            Times.Exactly(3)
        );
    }

    [Test]
    public void ShouldReturnTrueForNextDate()
    {
        var nextDate = new DateOnly(2022, 9, 1);
        var lastDate = new DateOnly(2022, 6, 1);
        var now = new DateTime(2022, 9, 1, 0, 0, 0);

        var result = _refresher.ItsTimeToRefresh(nextDate, lastDate, now);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void ShouldReturnFalseForNextDateEqPrevDate()
    {
        var nextDate = new DateOnly(2022, 9, 1);
        var lastDate = new DateOnly(2022, 9, 1);
        var now = new DateTime(2022, 9, 1, 0, 0, 0);

        var result = _refresher.ItsTimeToRefresh(nextDate, lastDate, now);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void ShouldReturnFalseForNowNotEqNextDate()
    {
        var nextDate = new DateOnly(2022, 9, 1);
        var lastDate = new DateOnly(2022, 9, 1);
        var now = new DateTime(2022, 8, 1, 0, 0, 0);

        var result = _refresher.ItsTimeToRefresh(nextDate, lastDate, now);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void ShouldReturnFalseForNowNotZeroHour()
    {
        var nextDate = new DateOnly(2022, 9, 1);
        var lastDate = new DateOnly(2022, 6, 1);
        var now = new DateTime(2022, 9, 1, 1, 0, 0);

        var result = _refresher.ItsTimeToRefresh(nextDate, lastDate, now);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void ShouldReturnNextUpdateDate()
    {
        var now = new DateTime(2022, 9, 2, 0, 0, 0);

        var result = _refresher.GetNextUpdateDate(now);
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 12, 1)));
    }

    [Test]
    public void ShouldReturnSameDate()
    {
        var now = new DateTime(2022, 9, 1, 0, 0, 0);

        var result = _refresher.GetNextUpdateDate(now);
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 9, 1)));
    }
    

    [Test]
    public async Task ShouldHandleInvalidToken()
    {
        var user = new User()
        {
            ChatId = 2517,
            Email = "a.pushkin@infinnity.ru"
        };

        _yandexAuthenticator
            .Setup(target => target.RefreshToken(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new YandexAuthenticationException("Refresh token expired"));

        await _refresher.RefreshToken(user);
        
        _userStorage.Verify(target => target.RemoveTokens(2517));
        _telegramClient.Verify(target => target.SendMessage(2517, 
            "У вас истек токен для отправки почты!\n\n" +
            $"Выполните команду /registermail для адреса a.pushkin@infinnity.ru"));
    }
}