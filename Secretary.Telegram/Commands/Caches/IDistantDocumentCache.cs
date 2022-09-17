using Secretary.Telegram.Commands.Caches.Interfaces;

namespace Secretary.Telegram.Commands.Caches;

public interface IDistantDocumentCache: IPeriodCache,
    IReasonCache,
    IFilePathCache, 
    IEmailsCache,
    IDocumentCreator,
    IMailCreator,
    IDocumentKeyCache
{
    
}