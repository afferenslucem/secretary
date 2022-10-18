using Moq;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Sessions;
using Secretary.Telegram.Wrappers;

namespace Secretary.Telegram.Tests.Wrappers;

public class SessionStorageWrapperTests
{
    private Mock<ISessionStorage> _sessionStorage = null!;
    private SessionStorageWrapper _wrapper = null!;

    [SetUp]
    public void Setup()
    {
        _sessionStorage = new Mock<ISessionStorage>();
        _wrapper = new SessionStorageWrapper(_sessionStorage.Object, 2517);
    }

    [Test]
    public async Task ShouldGetSession()
    {
        var expected = new Session(2517, new EmptyCommand());
        _sessionStorage.Setup(target => target.GetSession(2517)).ReturnsAsync(expected);
        
        var result = await _wrapper.GetSession();
        
        _sessionStorage.Verify(target => target.GetSession(2517), Times.Once);
        
        Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public async Task ShouldSaveSession()
    {
        var expected = new Session(2517, new EmptyCommand());
        
        await _wrapper.SaveSession(expected);
        
        _sessionStorage.Verify(target => target.SaveSession(2517, expected), Times.Once);
    }

    [Test]
    public async Task ShouldSaveCommandAsSession()
    {
        var expected = new EmptyCommand();
        
        await _wrapper.SaveSession(expected);
        
        _sessionStorage.Verify(target => target.SaveSession(2517, 
            It.Is<Session>(session => session.ChatId == 2517 && session.LastCommand == expected)), Times.Once);
    }

    [Test]
    public async Task ShouldDeleteSession()
    {
        await _wrapper.DeleteSession();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
    }
}