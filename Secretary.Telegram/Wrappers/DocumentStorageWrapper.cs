using Secretary.Storage;
using Secretary.Storage.Models;

namespace Secretary.Telegram.Wrappers;

public class DocumentStorageWrapper
{
    private IDocumentStorage _documentStorage;
    private long _chatId;

    public DocumentStorageWrapper(IDocumentStorage documentStorage, long chatId)
    {
        _documentStorage = documentStorage;
        _chatId = chatId;
    }

    public Task<Document> GetOrCreateDocument(string documentName)
    {
        return _documentStorage.GetOrCreateDocument(_chatId, documentName);
    }
}