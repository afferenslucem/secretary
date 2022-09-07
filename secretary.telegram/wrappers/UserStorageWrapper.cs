using secretary.storage;
using secretary.storage.models;

namespace secretary.telegram.wrappers;

public class UserStorageWrapper
{
    private IUserStorage _userStorage;
    private long _chatId;

    public UserStorageWrapper(IUserStorage userStorage, long chatId)
    {
        _userStorage = userStorage;
        _chatId = chatId;
    }

    public Task<User?> GetUser()
    {
        return _userStorage.GetUser(_chatId);
    }

    public Task SetUser(User user)
    {
        return _userStorage.SetUser(user);
    }
}