using Secretary.Storage.Models;

namespace Secretary.Telegram.Commands.Caches.Interfaces;

public interface IMailCreator
{
    string CreateMail(User user);
}