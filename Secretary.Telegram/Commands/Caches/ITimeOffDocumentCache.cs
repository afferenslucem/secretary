using Secretary.Telegram.Commands.Caches.Interfaces;

namespace Secretary.Telegram.Commands.Caches;

public interface ITimeOffDocumentCache: IPeriodCache,
    IReasonCache,
    IWorkingOffCache, 
    IFilePathCache, 
    IEmailsCache,
    IDocumentCreator,
    IMailCreator,
    IDocumentKeyCache
{
    
}