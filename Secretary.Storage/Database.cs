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

    public Database()
    {
        _userStorage = new UserStorage();
        _documentStorage = new DocumentStorage();
        _emailStorage = new EmailStorage();
    }
}