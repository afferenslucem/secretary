using Newtonsoft.Json;
using Secretary.Cache.JsonConverters;
using Secretary.Logging;
using Serilog;
using StackExchange.Redis;

namespace Secretary.Cache;

public class RedisCacheService : ICacheService
{
    private readonly ConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger _logger = LogPoint.GetLogger<RedisCacheService>();

    public RedisCacheService(string host)
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect(host);

        _logger.Information("Created RedisCacheService");
    }

    public async Task SaveEntity<T>(long key, T value, short? lifetimeSec) where T : class
    {
        await SaveEntity($"{typeof(T)}:{key}", value, lifetimeSec);
    }

    public async Task SaveEntity<T>(string key, T value, short? lifetimeSec) where T : class
    {
        try
        {
            _logger.Debug($"Save entity {key}");
            
            var db = _connectionMultiplexer.GetDatabase();

            var json = JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>() { new DateOnlyJsonConverter() },
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            var ttl = lifetimeSec != null ? TimeSpan.FromSeconds(lifetimeSec.Value) : (TimeSpan?) null;
            
            await db.StringSetAsync(new RedisKey(key), json, ttl);
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "Could not save entity");
            throw;
        }
    }

    public async Task<T?> GetEntity<T>(long key) where T : class
    {
        return await GetEntity<T>($"{typeof(T)}:{key}");
    }

    public async Task<T?> GetEntity<T>(string key) where T : class
    {
        try
        {
            _logger.Debug($"Ask entity {key}");
            
            var db = _connectionMultiplexer.GetDatabase();

            var redisValue = await db.StringGetAsync(new RedisKey(key));

            if (redisValue.IsNull) return null;

            var json = redisValue.ToString();

            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>() { new DateOnlyJsonConverter() },
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
            _logger.Debug($"Delete entity {typeof(T).Name}:{key}");
            
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