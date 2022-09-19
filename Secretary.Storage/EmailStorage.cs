using Microsoft.EntityFrameworkCore;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;

namespace Secretary.Storage;

public class EmailStorage: Storage, IEmailStorage
{
    public async Task SaveForDocument(long documentId, IEnumerable<Email> emails)
    {
        await using var context = GetContext();

        var oldEmails = context.Emails.Where(item => item.DocumentId == documentId);
        
        context.Emails.RemoveRange(oldEmails);
        
        foreach (var email in emails)
        {
            email.DocumentId = documentId;
        }

        await context.Emails.AddRangeAsync(emails);

        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Email>> GetForDocument(long documentId) 
    {
        await using var context = GetContext();

        var result = await context.Emails.Where(item => item.DocumentId == documentId).ToArrayAsync();

        return result;
    }
}