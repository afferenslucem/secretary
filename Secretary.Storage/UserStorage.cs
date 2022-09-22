using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Secretary.Logging;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Serilog;

namespace Secretary.Storage;

public class UserStorage: Storage, IUserStorage
{
    private ILogger _logger = LogPoint.GetLogger<UserStorage>();
    
    public async Task<User?> GetUser(long chatId)
    {
        await using var context = GetContext();

        var user =
            await context.Users.SingleOrDefaultAsync(item =>
                item.ChatId == chatId);

        return user;
    }

    public async Task SetUser(User user)
    {
        await using var context = GetContext();
        
        var exists = await context.Users.AnyAsync(item => item.ChatId == user.ChatId);
        _logger.Debug("Check entity existing");
        
        if (exists)
        {
            context.Update(user);
            _logger.Debug("Entity updated");
        }
        else
        {
            await context.Users.AddAsync(user);
            _logger.Debug("Entity inserted");
        }
        
        await context.SaveChangesAsync();
    }

    public async Task UpdateUser(User user)
    {
        await using var context = GetContext();
        
        _logger.Debug("Check entity existing");
 
        context.Update(user);
        
        _logger.Debug("Entity updated");
        
        await context.SaveChangesAsync();
    }

    public async Task RemoveTokens(long chatId)
    {
        await using var context = GetContext();

        var user = context.Users.Single(item => item.ChatId == chatId);

        user.AccessToken = null;
        user.RefreshToken = null;
        user.TokenCreationTime = null;
        user.TokenExpirationSeconds = null;
        
        await context.SaveChangesAsync();
    }

    public async Task<int> GetCount()
    {
        await using var context = GetContext();

        return await context.Users.CountAsync();
    }

    public async Task<int> GetCount(Expression<Func<User, bool>> predicate)
    {
        await using var context = GetContext();

        return await context.Users.CountAsync(predicate);
    }

    public async Task<int> GetCountWithDocuments()
    {
        await using var context = GetContext();

        return await context.Users
            .Include(user => user.Documents)
            .Where(user => user.Documents.Count() > 0)
            .CountAsync();
    }

    public async Task<User[]> GetUsers(int from, int length)
    {
        await using var context = GetContext();
        
        return await context.Users.Skip(from).Take(length).ToArrayAsync();
    }

    public async Task<User[]> GetUsers(Expression<Func<User, bool>> predicate)
    {
        await using var context = GetContext();
        
        return await context.Users.Where(predicate).ToArrayAsync();
    }

    public async Task<User[]> GetUsers(int from, int length, Expression<Func<User, bool>> predicate)
    {
        await using var context = GetContext();
        
        return await context.Users
            .Where(predicate)
            .OrderBy(user => user.ChatId)
            .Skip(from).Take(length)
            .ToArrayAsync();
    }
}