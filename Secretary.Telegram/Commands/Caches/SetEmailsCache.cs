using Secretary.Storage.Models;
using Secretary.Telegram.Commands.Caches.Interfaces;

namespace Secretary.Telegram.Commands.Caches;

public class SetEmailsCache : IDocumentKeyCache, IEmailsCache
{
    public string DocumentKey { get; set; } = null!;
    public IEnumerable<Email>? Emails { get; set; } = null!;
}