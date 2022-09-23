using Secretary.Cache;
using Secretary.HealthCheck.Data;

namespace Secretary.HealthCheck;

public class HealthCheckService
{
    private ICacheService _cacheService;

    public HealthCheckService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public Task SaveData<T>(T data) where T: class
    {
        return _cacheService.SaveEntity(typeof(T).Name, data, 7 * 24 * 60 * 60);
    }

    public Task<T?> GetData<T>() where T: class
    {
        return _cacheService.GetEntity<T>(typeof(T).Name);
    }

    public async Task<HealthData> GetFullData()
    {
        var botHealthData = await GetData<BotHealthData>();
        var reminderHealthData = await GetData<ReminderHealthData>();
        var refresherHealthData = await GetData<RefresherHealthData>();

        return new HealthData()
        {
            BotHealthData = botHealthData,
            RefresherHealthData = refresherHealthData,
            ReminderHealthData = reminderHealthData
        };
    }
}