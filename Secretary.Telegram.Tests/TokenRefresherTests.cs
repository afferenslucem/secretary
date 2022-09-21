using Moq;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Exceptions;

namespace Secretary.Telegram.Tests;

public class TokenRefresherTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<IYandexAuthenticator> _yandexAuthenticator = null!;
    
    public TokenRefresher _refresher = null!;

    [SetUp]
    public void Setup()
    {
        _yandexAuthenticator = new Mock<IYandexAuthenticator>();
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();
        
        _refresher = new TokenRefresher(
            _yandexAuthenticator.Object, 
            _client.Object, 
            _userStorage.Object, 
            CancellationToken.None
        );
    }

    [Test]
    public async Task ShouldFireTokenExpired()
    {
        var fired = false;

        _refresher.OnUserInvalidToken += (User user) =>
        {
            fired = true; return Task.CompletedTask; 
        };

        var user = new User();

        _yandexAuthenticator
            .Setup(
                target => target.RefreshToken(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            ).Throws(new YandexAuthenticationException("Refresh token expired"));

        Assert.ThrowsAsync<YandexAuthenticationException>(() => _refresher.RefreshToken(user));
        
        Assert.That(fired, Is.True);
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
}