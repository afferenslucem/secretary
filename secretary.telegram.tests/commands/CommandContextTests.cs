using Moq;
using secretary.telegram.commands;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands;

public class CommandContextTests
{
    private Mock<ISessionStorage> _storage = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._storage = new Mock<ISessionStorage>();

        this._context = new CommandContext();

        _context.SessionStorage = this._storage.Object;
    }
    
    [Test]
    public async Task ShouldFindSessionForChatId()
    {
        _context.ChatId = 2517;
        
        _storage.Setup(obj => obj.GetSession(It.IsAny<long>())).ReturnsAsync((Session?)null);

        var result = await this._context.GetSession();
        
        this._storage.Verify(target => target.GetSession(2517));
    }
    
    [Test]
    public async Task ShouldSaveSession()
    {
        _context.ChatId = 2517;
        
        _storage.Setup(obj => obj.SaveSession(It.IsAny<long>(), It.IsAny<Session>()));

        await this._context.SaveSession(null!);
        
        this._storage.Verify(target => target.SaveSession(2517, It.IsAny<Session>()));
    }
    
    [Test]
    public void ShouldThrowSessionStorageIsNullForGetSession()
    {
        _context.SessionStorage = null!;
        
        Assert.ThrowsAsync<NullReferenceException>(async () => await this._context.GetSession());
    }
    
    [Test]
    public void ShouldThrowSessionStorageIsNullForSaveSession()
    {
        _context.SessionStorage = null!;
        
        Assert.ThrowsAsync<NullReferenceException>(async () => await this._context.SaveSession(null!));
    }
}