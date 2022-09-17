using Microsoft.EntityFrameworkCore;
using Secretary.Storage.Models;

namespace Secretary.Storage;

public class UserStorage: Storage, IUserStorage
{
    public async Task<User?> GetUser(long chatId)
    {
        await using var context = GetContext();

        var user =
            await context.Users.FirstOrDefaultAsync(item =>
                item.ChatId == chatId);

        return user;
    }

    public async Task SetUser(User user)
    {
        var data = await this.GetUser(user.ChatId);

        if (data == null)
        {
            await this.InsertUser(user);
        }
        else
        {
            await UpdateUser(user);
        }
    }

    private async Task InsertUser(User user)
    {
        await using var context = GetContext();

        await context.Users.AddAsync(user);

        await context.SaveChangesAsync();
    }

    private async Task UpdateUser(User user)
    {
        await using var context = GetContext();

        context.Users.Update(user);

        await context.SaveChangesAsync();
    }
}