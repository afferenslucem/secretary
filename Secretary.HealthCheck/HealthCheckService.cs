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

    public Task SaveData(HealthData data)
    {
        return _cacheService.SaveEntity(typeof(HealthData).Name, data, null);
    }

    public Task<HealthData?> GetData()
    {
        return _cacheService.GetEntity<HealthData>(typeof(HealthData).Name);
    }
}