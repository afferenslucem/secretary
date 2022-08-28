using Moq;
using secretary.telegram.commands;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands;

public class CommandContextTests
{
    private Mock<ISessionStorage> storage;
    private CommandContext context;
        
    [SetUp]
    public void Setup()
    {
        this.storage = new Mock<ISessionStorage>();

        this.context = new CommandContext();

        context.SessionStorage = this.storage.Object;
    }
    
    [Test]
    public async Task ShouldFindSessionForChatId()
    {
        context.ChatId = 2517;
        
        storage.Setup(obj => obj.GetSession(It.IsAny<long>())).ReturnsAsync((Session?)null);

        var result = await this.context.GetSession();
        
        this.storage.Verify(target => target.GetSession(2517));
    }
    
    [Test]
    public async Task ShouldSaveSession()
    {
        context.ChatId = 2517;
        
        storage.Setup(obj => obj.SaveSession(It.IsAny<long>(), It.IsAny<Session>()));

        await this.context.SaveSession(null);
        
        this.storage.Verify(target => target.SaveSession(2517, It.IsAny<Session>()));
    }
    
    [Test]
    public void ShouldThrowSessionStorageIsNullForGetSession()
    {
        context.SessionStorage = null;
        
        Assert.ThrowsAsync<NullReferenceException>(async () => await this.context.GetSession());
    }
    
    [Test]
    public void ShouldThrowSessionStorageIsNullForSaveSession()
    {
        context.SessionStorage = null;
        
        Assert.ThrowsAsync<NullReferenceException>(async () => await this.context.SaveSession(null));
    }
}