using System.Data.SQLite;

namespace Secretary.Storage;

public abstract class Storage
{ 
    public string DbFile { get; private set; }
    
    public Storage(string dbFile)
    {
        DbFile = dbFile;
    }
    
    protected SQLiteConnection GetConnection()
    {
        return new SQLiteConnection("Data Source=" + DbFile);
    }
}