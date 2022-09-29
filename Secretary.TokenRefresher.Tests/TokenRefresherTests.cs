using Moq;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Exceptions;

namespace Secretary.TokenRefresher.Tests;

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
        var user2 = new User()
        {
            RefreshToken = "refresh_token"
        };

        _userStorage.Setup(target => target.GetUsers(user => user.RefreshToken != null))
            .ReturnsAsync(new[] {  user2 });
        
        await _refresher.RefreshTokensForAllUsers();
        
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
    public void ShouldReturnNextUpdateDate()
    {
        var now = new DateTime(2022, 9, 2, 0, 0, 0);

        var result = _refresher.GetNextUpdateDate(now);
        
        Assert.That(result == new DateTime(2022, 12, 1).ToUniversalTime(), Is.True);
    }

    [Test]
    public void ShouldReturnSameDate()
    {
        var now = new DateTime(2022, 9, 1, 0, 0, 0);

        var result = _refresher.GetNextUpdateDate(now);
        
        Assert.That(result == new DateTime(2022, 9, 1).ToUniversalTime(), Is.True);
    }

    [Test]
    public void ShouldReturnNextYear()
    {
        var now = new DateTime(2022, 12, 2, 0, 0, 0);

        var result = _refresher.GetNextUpdateDate(now);
        
        Assert.That(result == new DateTime(2023, 3, 1).ToUniversalTime(), Is.True);
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
    
    [Test]
    public void ShouldReturnHealthData()
    {
        _refresher.LastActivityDateCheck = DateTime.Now.AddMinutes(-1);

        var result = _refresher.GetHealthData();
        
        Assert.That(result.Version, Is.EqualTo(TokenRefresher.Version));
        Assert.That(result.DeployTime, Is.EqualTo(TokenRefresher.Uptime));
        Assert.That(result.PingTime, Is.EqualTo(_refresher.LastActivityDateCheck));
        Assert.That(result.NextRefreshDate == _refresher.NextRefreshDate, Is.True);
    }
}