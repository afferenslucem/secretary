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

    public async Task<int> GetCount()
    {
        await using var context = GetContext();

        return await context.Users.CountAsync();
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
}