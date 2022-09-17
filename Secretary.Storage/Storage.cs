using System.Data.SQLite;
using Secretary.Storage.Models;

namespace Secretary.Storage;

public abstract class Storage
{
    protected DatabaseContext GetContext()
    {
        return new DatabaseContext();
    }
}