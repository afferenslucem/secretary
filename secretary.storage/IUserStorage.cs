using secretary.storage.models;

namespace secretary.storage;

public interface IUserStorage
{
    Task<User?> GetUser(long chatId);
    Task SetUser(User user);
}