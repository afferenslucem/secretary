using secretary.storage.models;

namespace secretary.storage;

public interface IEmailStorage
{
    Task SaveForDocument(long documentId, IEnumerable<Email> emails);
    Task<IEnumerable<Email>> GetForDocument(long documentId);
    Task<long> GetCountForDocument(long documentId);
}