using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using secretary.logging;
using StackExchange.Redis;

namespace secretary.cache;

public class RedisCacheService: ICacheService
{
    private ILogger<RedisCacheService> _logger = LogPoint.GetLogger<RedisCacheService>();

    private ConnectionMultiplexer _connectionMultiplexer;

    public RedisCacheService(string host)
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect(host);
        
        _logger.LogInformation("Created RedisCacheService");
    }

    public async Task SaveEntity<T>(long key, T value, short lifetimeSec = 900) where T : class
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();

            var json = JsonSerializer.Serialize(value);

            await db.StringSetAsync(new RedisKey($"{typeof(T)}:{key}"), json, TimeSpan.FromSeconds(lifetimeSec));
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Could not save entity");
            throw;
        }
    }

    public async Task<T?> GetEntity<T>(long key) where T : class
    {
        try {
            var db = _connectionMultiplexer.GetDatabase();

            var redisValue = await db.StringGetAsync(new RedisKey($"{typeof(T)}:{key}"));

            if (redisValue.IsNull)
            {
                return null;
            }

            var json = redisValue.ToString();
            
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Could not get entity");
            throw;
        }
    }

    public async Task DeleteEntity<T>(long key) where T : class
    {
        try {
            var db = _connectionMultiplexer.GetDatabase();

            await db.KeyDeleteAsync(new RedisKey($"{typeof(T)}:{key}"));
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Could not delete key");
            throw;
        }
    }
}