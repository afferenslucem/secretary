namespace Secretary.Storage;

public class Database
{
    public string DbFile { get; private set; }
    
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

    public Database(string dbFile)
    {
        DbFile = dbFile;

        this._userStorage = new UserStorage(dbFile);
        this._documentStorage = new DocumentStorage(dbFile);
        this._emailStorage = new EmailStorage(dbFile);
    }
    
    public void InitDb()
    {
        if (!File.Exists(DbFile))
        {
            _userStorage.CreateTable();
            _documentStorage.CreateTable();
            _emailStorage.CreateTable();
        }
    }
}