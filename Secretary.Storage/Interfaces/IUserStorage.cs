using Secretary.Storage.Models;

namespace Secretary.Storage.Interfaces;

public interface IUserStorage
{
    Task<User?> GetUser(long chatId);
    Task SetUser(User user);
    Task RemoveTokens(long chatId);
    Task<int> GetCount();
    Task<int> GetCountWithDocuments();

    Task<User[]> GetUsers(int from, int length);
}