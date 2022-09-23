using Moq;
using Secretary.Cache;
using Secretary.HealthCheck.Data;

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
        var data = new BotHealthData();

        await _service.SaveData(data);
        
        _cacheService.Verify(target => target.SaveEntity("BotHealthData", data, 604_800));
    }

    [Test]
    public async Task ShouldGetData()
    {
        var data = new BotHealthData();
        
        _cacheService.Setup(target => target.GetEntity<BotHealthData>("BotHealthData")).ReturnsAsync(data);

        var result = await _service.GetData<BotHealthData>();
        
        Assert.That(result, Is.SameAs(data));
    }

    [Test]
    public async Task ShouldGetFullData()
    {
        var botHealthData = new BotHealthData();
        var reminderHealthData = new ReminderHealthData();
        var refresherHealthData = new RefresherHealthData();
        
        
        _cacheService.Setup(target => target.GetEntity<BotHealthData>("BotHealthData")).ReturnsAsync(botHealthData);
        _cacheService.Setup(target => target.GetEntity<ReminderHealthData>("ReminderHealthData")).ReturnsAsync(reminderHealthData);
        _cacheService.Setup(target => target.GetEntity<RefresherHealthData>("RefresherHealthData")).ReturnsAsync(refresherHealthData);

        var result = await _service.GetFullData();
        
        Assert.That(result.BotHealthData, Is.SameAs(botHealthData));
        Assert.That(result.RefresherHealthData, Is.SameAs(refresherHealthData));
        Assert.That(result.ReminderHealthData, Is.SameAs(reminderHealthData));
    }
}