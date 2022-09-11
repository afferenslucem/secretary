using Newtonsoft.Json;
using secretary.logging;
using Serilog;
using StackExchange.Redis;

namespace secretary.cache;

public class RedisCacheService: ICacheService
{
    private ILogger _logger = LogPoint.GetLogger<RedisCacheService>();

    private ConnectionMultiplexer _connectionMultiplexer;

    public RedisCacheService(string host)
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect(host);
        
        _logger.Information("Created RedisCacheService");
    }

    public async Task SaveEntity<T>(long key, T value, short lifetimeSec) where T : class
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();

            var json = JsonConvert.SerializeObject(value, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            await db.StringSetAsync(new RedisKey($"{typeof(T)}:{key}"), json, TimeSpan.FromSeconds(lifetimeSec));
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "Could not save entity");
            throw;
        }
    }

    public async Task<T?> GetEntity<T>(long key) where T : class
    {
        try 
        {
            var db = _connectionMultiplexer.GetDatabase();

            var redisValue = await db.StringGetAsync(new RedisKey($"{typeof(T)}:{key}"));

            if (redisValue.IsNull)
            {
                return null;
            }

            var json = redisValue.ToString();
            
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "Could not get entity");
            throw;
        }
    }

    public async Task DeleteEntity<T>(long key) where T : class
    {
        try 
        {
            var db = _connectionMultiplexer.GetDatabase();

            await db.KeyDeleteAsync(new RedisKey($"{typeof(T)}:{key}"));
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "Could not delete key");
            throw;
        }
    }
}