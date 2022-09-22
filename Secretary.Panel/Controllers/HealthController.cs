using Microsoft.AspNetCore.Mvc;
using Secreatry.HealthCheck;
using Secretary.Cache;
using Secretary.Configuration;
using Secretary.HealthCheck;
using Secretary.HealthCheck.Data;

namespace Secretary.Panel.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    public HealthCheckService HealthCheckService;

    public HealthController()
    {
        var cacheService = new RedisCacheService(Config.Instance.RedisHost);
        HealthCheckService = new(cacheService);
    }

    [HttpGet(Name = "GetHealthData")]
    public async Task<HealthData> GetData()
    {
        var result = await HealthCheckService.GetData();

        return result;
    }
}