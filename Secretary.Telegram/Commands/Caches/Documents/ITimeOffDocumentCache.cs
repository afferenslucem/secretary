using Secretary.Telegram.Commands.Caches.Documents.Interfaces;

namespace Secretary.Telegram.Commands.Caches.Documents;

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