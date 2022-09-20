using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;

namespace Secretary.Telegram.Wrappers;

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

    public Task RemoveTokens()
    {
        return _userStorage.RemoveTokens(_chatId);
    }
}