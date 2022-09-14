using Secretary.Cache;

namespace Secretary.Telegram.Sessions;

public class SessionStorage: ISessionStorage
{
    private readonly ICacheService _cacheService;

    public SessionStorage(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<Session?> GetSession(long chatId)
    {
        var result = await _cacheService.GetEntity<Session>(chatId);

        return result;
    }

    public async Task SaveSession(long chatId, Session session)
    {
        await _cacheService.SaveEntity(chatId, session);
    }

    public async Task DeleteSession(long chatId)
    {
        await _cacheService.DeleteEntity<Session>(chatId);
    }
}