using Secretary.Storage.Models;

namespace Secretary.Storage;

public interface IDocumentStorage
{
    Task<Document> GetOrCreateDocument(long chatId, string documentName);
}