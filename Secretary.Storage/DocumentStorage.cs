using Microsoft.EntityFrameworkCore;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;

namespace Secretary.Storage;

public class DocumentStorage: Storage, IDocumentStorage
{
    public async Task<Document> GetOrCreateDocument(long chatId, string documentName)
    {
        var document = await GetDocument(chatId, documentName);

        if (document != null) return document;

        document = await this.SaveDocument(chatId, documentName);

        return document;
    }

    private async Task<Document?> GetDocument(long chatId, string documentName)
    {
        await using var context = GetContext();

        var document =
            await context.Documents.FirstOrDefaultAsync(item =>
                item.UserChatId == chatId && item.DocumentName == documentName);

        return document;
    }

    private async Task<Document> SaveDocument(long chatId, string documentName)
    {
        var document = new Document(chatId, documentName);
        
        await using var context = GetContext();

        context.Documents.Add(document);

        await context.SaveChangesAsync();

        return document;
    }
}