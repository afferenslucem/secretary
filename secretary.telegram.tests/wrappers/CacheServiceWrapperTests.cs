using Moq;
using secretary.cache;
using secretary.telegram.wrappers;

namespace secretary.telegram.tests.wrappers;

public class CacheServiceWrapperTests
{
    private Mock<ICacheService> _cacheService = null!;
    private CacheServiceWrapper _wrapper = null!;
    
    [SetUp]
    public void Setup()
    {
        _cacheService = new Mock<ICacheService>();
        _wrapper = new CacheServiceWrapper(_cacheService.Object, 2517);
    }

    [Test]
    public async Task ShouldSaveEntity()
    {
        _cacheService.Setup(target => target.SaveEntity(2517, "/timeoff", 600));

        await _wrapper.SaveEntity("/timeoff");
        
        _cacheService.Verify(target => target.SaveEntity(2517, "/timeoff", 600), Times.Once);
    }

    [Test]
    public async Task ShouldGetEntity()
    {
        var expected = "expected";
        
        _cacheService.Setup(target => target.GetEntity<string>(2517)).ReturnsAsync(expected);

        var result = await _wrapper.GetEntity<string>();
        
        _cacheService.Verify(target => target.GetEntity<string>(2517), Times.Once);
        
        Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public async Task ShouldDeleteEntity()
    {
        _cacheService.Setup(target => target.DeleteEntity<string>(2517));

        await _wrapper.DeleteEntity<string>();
        
        _cacheService.Verify(target => target.DeleteEntity<string>(2517), Times.Once);
    }
}