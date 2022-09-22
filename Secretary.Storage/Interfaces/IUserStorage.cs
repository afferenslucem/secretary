using System.Linq.Expressions;
using Secretary.Storage.Models;

namespace Secretary.Storage.Interfaces;

public interface IUserStorage
{
    Task<User?> GetUser(long chatId);
    Task SetUser(User user);
    Task UpdateUser(User user);
    Task RemoveTokens(long chatId);
    Task<int> GetCount();
    Task<int> GetCount(Expression<Func<User, bool>> predicate);
    Task<int> GetCountWithDocuments();

    Task<User[]> GetUsers(int from, int length);
    Task<User[]> GetUsers(int from, int length, Expression<Func<User, bool>> predicate);
    Task<User[]> GetUsers(Expression<Func<User, bool>> predicate);
}