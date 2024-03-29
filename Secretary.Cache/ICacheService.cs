﻿namespace Secretary.Cache;

public interface ICacheService
{
    Task SaveEntity<T>(long key, T value, int? lifetimeSec = 600) where T : class;
    Task SaveEntity<T>(string key, T value, int? lifetimeSec = 600) where T : class;
    Task<T?> GetEntity<T>(long key) where T : class;
    Task<T?> GetEntity<T>(string key) where T : class;
    Task DeleteEntity<T>(long key) where T : class;
}