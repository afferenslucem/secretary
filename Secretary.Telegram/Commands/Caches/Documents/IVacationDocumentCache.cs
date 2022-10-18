using Secretary.Telegram.Commands.Caches.Documents.Interfaces;

namespace Secretary.Telegram.Commands.Caches.Documents;

public interface IVacationDocumentCache: IPeriodCache,
    IFilePathCache, 
    IEmailsCache,
    IDocumentCreator,
    IMailCreator,
    IDocumentKeyCache
{
    
}