using Microsoft.EntityFrameworkCore;
using Secretary.Logging;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Serilog;

namespace Secretary.Storage;

public class Database
{
    private ILogger _logger = LogPoint.GetLogger<Database>();
    
    private UserStorage _userStorage;
    private DocumentStorage _documentStorage;
    private EmailStorage _emailStorage;
    private EventLogStorage _eventLogStorage;

    public virtual IUserStorage UserStorage
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
    
    public IEventLogStorage EventLogStorage
    {
        get => _eventLogStorage;
    }

    public Database()
    {
        _userStorage = new UserStorage();
        _documentStorage = new DocumentStorage();
        _emailStorage = new EmailStorage();
        _eventLogStorage = new EventLogStorage();
    }

    public void MigrateDatabase()
    {
        try
        {
            _logger.Information("Run migrations");
            using var context = new DatabaseContext();
            context.Database.Migrate();
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "Could not run migrations");

            throw;
        }
    }
}