using secretary.storage.models;

namespace secretary.storage;

public interface IDocumentStorage
{
    Task<Document> GetOrCreateDocument(long chatId, string documentName);
}