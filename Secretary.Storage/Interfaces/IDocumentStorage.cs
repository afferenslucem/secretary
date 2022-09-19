using Secretary.Storage.Models;

namespace Secretary.Storage.Interfaces;

public interface IDocumentStorage
{
    Task<Document> GetOrCreateDocument(long chatId, string documentName);
}