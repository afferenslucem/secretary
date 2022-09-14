using Secretary.Telegram.Commands.Caches.Interfaces;

namespace Secretary.Telegram.Commands.Caches;

public interface IVacationDocumentCache: IPeriodCache,
    IFilePathCache, 
    IEmailsCache,
    IDocumentCreator,
    IMailCreator,
    IDocumentKeyCache
{
    
}