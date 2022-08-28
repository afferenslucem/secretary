using secretary.storage.models;

namespace secretary.storage;

public interface IUserStorage
{
    Task<User?> GetUser(long chatId);
    Task InsertUser(User user);
    Task UpdateUser(User user);
    Task SetUser(User user);
}