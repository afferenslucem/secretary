namespace secretary.telegram.sessions;

public interface ISessionStorage
{
    Task<Session?> GetSession(long chatId);
    Task SaveSession(long chatId, Session session);
}