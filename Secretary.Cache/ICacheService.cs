namespace Secretary.Cache;

public interface ICacheService
{
    Task SaveEntity<T>(long key, T value, short lifetimeSec = 600) where T : class;
    Task<T?> GetEntity<T>(long key) where T : class;
    Task DeleteEntity<T>(long key) where T : class;
}