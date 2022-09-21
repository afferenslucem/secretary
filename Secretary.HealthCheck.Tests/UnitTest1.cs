using Moq;
using Secreatry.HealthCheck;
using Secreatry.HealthCheck.Data;
using Secretary.Cache;

namespace Secretary.HealthCheck.Tests;

public class HealthCheckServiceTests
{
    private Mock<ICacheService> _cacheService = null!;
    private HealthCheckService _service = null!;
    
    [SetUp]
    public void Setup()
    {
        _cacheService = new Mock<ICacheService>();
        _service = new HealthCheckService(_cacheService.Object);
    }

    [Test]
    public async Task ShouldSaveData()
    {
        var data = new HealthData();

        await _service.SaveData(data);
        
        _cacheService.Verify(target => target.SaveEntity("HealthData", data, null));
    }

    [Test]
    public async Task ShouldGetData()
    {
        var data = new HealthData();
        
        _cacheService.Setup(target => target.GetEntity<HealthData>("HealthData")).ReturnsAsync(data);

        var result = await _service.GetData();
        
        Assert.That(result, Is.SameAs(data));
    }
}