using Dapper;
using secretary.storage.models;

namespace secretary.storage;

public class UserStorage: Storage, IUserStorage
{
    
    public UserStorage(string dbFile): base(dbFile)
    {
    }

    public async Task<User?> GetUser(long chatId)
    {
        using var connection = this.GetConnection();

        await connection.OpenAsync();

        var query = await connection.QueryAsync<User>(
            @"select ChatId, Name, NameGenitive, JobTitle, JobTitleGenitive, Email, AccessToken, RefreshToken from Users where ChatId = @chatId;", new { chatId });

        var result = query.FirstOrDefault();

        return result;
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

    public async Task InsertUser(User user)
    {
        using var connection = this.GetConnection();

        await connection.OpenAsync();

        await connection.ExecuteAsync(
            @"insert into Users 
                (ChatId, Name, NameGenitive, JobTitle, JobTitleGenitive, Email, AccessToken, RefreshToken) values 
                (@ChatId, @Name, @NameGenitive, @JobTitle, @JobTitleGenitive, @Email, @AccessToken, @RefreshToken);",
            user);
    }

    public async Task UpdateUser(User user)
    {
        using var connection = this.GetConnection();

        await connection.OpenAsync();

        await connection.ExecuteAsync(
            @"update Users 
                set Name = @Name,
                    NameGenitive = @NameGenitive,
                    JobTitle = @JobTitle,
                    JobTitleGenitive = @JobTitleGenitive,
                    Email = @Email,
                    AccessToken = @AccessToken,
                    RefreshToken = @RefreshToken
                where ChatId = @ChatId;",
            user);
    }
    
    public void CreateTable()
    {
        using var connection = GetConnection();
        
        connection.Open();
        connection.Execute(
            @"create table Users
            (
                ChatId              integer identity primary key,
                Name                varchar(128),
                NameGenitive        varchar(128),
                JobTitle            varchar(128),
                JobTitleGenitive    varchar(128),
                Email    varchar(128),
                AccessToken         varchar(128),
                RefreshToken        varchar(256)
            )");
    }
}