using Secretary.Storage.Models;

namespace Secretary.Telegram.Commands.Caches.Documents.Interfaces;

public interface IEmailsCache
{
    IEnumerable<Email>? Emails { get; set; }
}