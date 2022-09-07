using secretary.telegram.commands;
using secretary.telegram.sessions;

namespace secretary.telegram.wrappers;

public class SessionStorageWrapper
{
    private ISessionStorage _sessionStorage;
    private long _chatId;

    public SessionStorageWrapper(ISessionStorage sessionStorage, long chatId)
    {
        _chatId = chatId;
        _sessionStorage = sessionStorage;
    }

    public Task<Session?> GetSession()
    {
        return _sessionStorage.GetSession(_chatId);
    }

    public Task SaveSession(Session session)
    {
        return _sessionStorage.SaveSession(_chatId, session);
    }

    public Task SaveSession(Command command)
    {
        return _sessionStorage.SaveSession(_chatId, new Session(_chatId, command));
    }

    public Task DeleteSession()
    {
        return _sessionStorage.DeleteSession(_chatId);
    }
}