using Secretary.Storage.Models;

namespace Secretary.Storage;

public interface IUserStorage
{
    Task<User?> GetUser(long chatId);
    Task SetUser(User user);
}