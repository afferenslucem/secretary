using Secretary.Storage.Models;

namespace Secretary.Telegram.Commands.Caches.Interfaces;

public interface IDocumentCreator
{
    string CreateDocument(User user);
}