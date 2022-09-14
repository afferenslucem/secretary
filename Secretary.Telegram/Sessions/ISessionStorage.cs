namespace Secretary.Telegram.Sessions;

public interface ISessionStorage
{
    Task<Session?> GetSession(long chatId);
    Task SaveSession(long chatId, Session session);
    Task DeleteSession(long chatId);
}