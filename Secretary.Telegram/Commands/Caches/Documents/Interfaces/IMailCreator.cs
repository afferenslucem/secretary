using Secretary.Storage.Models;

namespace Secretary.Telegram.Commands.Caches.Documents.Interfaces;

public interface IMailCreator
{
    string CreateMail(User user);
}