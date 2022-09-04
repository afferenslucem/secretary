namespace secretary.cache;

public interface ICacheService
{
    Task SaveEntity<T>(long key, T value, short lifetimeSec = 900) where T : class;
    Task<T?> GetEntity<T>(long key) where T : class;
    Task DeleteEntity<T>(long key) where T : class;
}