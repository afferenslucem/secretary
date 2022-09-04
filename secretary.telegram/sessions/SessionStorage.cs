using secretary.cache;

namespace secretary.telegram.sessions;

public class SessionStorage: ISessionStorage
{
    private readonly ICacheService _cacheService;

    public SessionStorage()
    {
        _cacheService = new RedisCacheService("localhost:6379");
    }

    public async Task<Session?> GetSession(long chatId)
    {
        var result = await _cacheService.GetEntity<Session>(chatId);

        return result;
    }

    public async Task SaveSession(long chatId, Session session)
    {
        await _cacheService.SaveEntity<Session>(chatId, session);
    }

    public async Task DeleteSession(long chatId)
    {
        await _cacheService.DeleteEntity<Session>(chatId);
    }
}