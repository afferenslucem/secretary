using Moq;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Wrappers;

namespace Secretary.Telegram.Tests.Wrappers;

public class UserStorageWrapperTests
{
    private Mock<IUserStorage> _userStorage = null!;
    private UserStorageWrapper _wrapper = null!;
    
    [SetUp]
    public void Setup()
    {
        _userStorage = new Mock<IUserStorage>();
        _wrapper = new UserStorageWrapper(_userStorage.Object, 2517);
    }

    [Test]
    public async Task ShouldGetUser()
    {
        var expected = new User();

        _userStorage.Setup(target => target.GetUser(2517)).ReturnsAsync(expected);

        var result = await _wrapper.GetUser();
        
        _userStorage.Verify(target => target.GetUser(2517), Times.Once);
        
        Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public async Task ShouldSetUser()
    {
        var expected = new User();

        await _wrapper.SetUser(expected);
        
        _userStorage.Verify(target => target.SetUser(expected), Times.Once);
    }
}