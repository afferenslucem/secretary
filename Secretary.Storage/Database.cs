using Microsoft.EntityFrameworkCore;
using Secretary.Storage.Models;

namespace Secretary.Storage;

public class Database
{
    private UserStorage _userStorage;
    private DocumentStorage _documentStorage;
    private EmailStorage _emailStorage;

    public IUserStorage UserStorage
    {
        get => _userStorage;
    }
    
    public IDocumentStorage DocumentStorage
    {
        get => _documentStorage;
    }
    
    public IEmailStorage EmailStorage
    {
        get => _emailStorage;
    }

    public Database()
    {
        this.MigrateDatabase();

        _userStorage = new UserStorage();
        _documentStorage = new DocumentStorage();
        _emailStorage = new EmailStorage();
    }

    private void MigrateDatabase()
    {
        using var context = new DatabaseContext();
        context.Database.Migrate();
    }
}