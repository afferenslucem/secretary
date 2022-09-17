using Secretary.Storage.Models;

namespace Secretary.Storage;

public interface IEmailStorage
{
    Task SaveForDocument(long documentId, IEnumerable<Email> emails);
    Task<IEnumerable<Email>> GetForDocument(long documentId);
}