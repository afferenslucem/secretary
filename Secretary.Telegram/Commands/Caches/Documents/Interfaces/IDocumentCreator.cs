using Secretary.Storage.Models;

namespace Secretary.Telegram.Commands.Caches.Documents.Interfaces;

public interface IDocumentCreator
{
    string CreateDocument(User user);
}