using Microsoft.AspNetCore.Mvc;
using Secretary.Cache;
using Secretary.Configuration;
using Secretary.HealthCheck.Data;
using Secretary.HealthCheck;

namespace Secretary.Panel.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    public readonly HealthCheckService HealthCheckService;

    public HealthController()
    {
        var cacheService = new RedisCacheService(Config.Instance.RedisHost);
        HealthCheckService = new(cacheService);
    }

    [HttpGet(Name = "GetHealthData")]
    public async Task<HealthData> GetData()
    {
        var result = await HealthCheckService.GetFullData();

        return result;
    }
}