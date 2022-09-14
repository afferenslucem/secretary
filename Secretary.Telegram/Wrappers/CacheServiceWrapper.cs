using Secretary.Cache;

namespace Secretary.Telegram.Wrappers;

public class CacheServiceWrapper
{
    private ICacheService _cacheService;
    private long _chatId;
    
    public CacheServiceWrapper(ICacheService cacheService, long chatId)
    {
        _cacheService = cacheService;
        _chatId = chatId;
    }

    public Task SaveEntity<T>(T value, short lifetimeSec = 600) where T : class
    {
        return _cacheService.SaveEntity(_chatId, value, lifetimeSec);
    }

    public Task<T?> GetEntity<T>() where T : class
    {
        return _cacheService.GetEntity<T>(_chatId);
    }

    public Task DeleteEntity<T>() where T : class
    {
        return _cacheService.DeleteEntity<T>(_chatId);
    }
}