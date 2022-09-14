using Secretary.Storage.Models;

namespace Secretary.Telegram.Commands.Caches.Interfaces;

public interface IEmailsCache
{
    IEnumerable<Email>? Emails { get; set; }
}