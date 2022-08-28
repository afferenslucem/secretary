namespace secretary.telegram.sessions;

public class SessionStorage: ISessionStorage
{
    private readonly Dictionary<long, Session> sessions = new Dictionary<long, Session>();

    public Task<Session?> GetSession(long chatId)
    {
        Session? result;

        return Task.Run(() => sessions.TryGetValue(chatId, out result) ? result : null);
    }

    public Task SaveSession(long chatId, Session session)
    {
        this.sessions[chatId] = session;
        
        return Task.CompletedTask;
    }
}