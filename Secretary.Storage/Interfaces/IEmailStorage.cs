using Secretary.Storage.Models;

namespace Secretary.Storage.Interfaces;

public interface IEmailStorage
{
    Task SaveForDocument(long documentId, IEnumerable<Email> emails);
    Task<IEnumerable<Email>> GetForDocument(long documentId);
}