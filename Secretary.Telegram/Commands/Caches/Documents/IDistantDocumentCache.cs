using Secretary.Telegram.Commands.Caches.Documents.Interfaces;

namespace Secretary.Telegram.Commands.Caches.Documents;

public interface IDistantDocumentCache: IPeriodCache,
    IReasonCache,
    IFilePathCache, 
    IEmailsCache,
    IDocumentCreator,
    IMailCreator,
    IDocumentKeyCache
{
    
}