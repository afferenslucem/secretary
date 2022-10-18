using Secretary.Storage.Models;
using Secretary.Telegram.Commands.Caches.Documents.Interfaces;

namespace Secretary.Telegram.Commands.Caches.Documents;

public class SetEmailsCache : IDocumentKeyCache, IEmailsCache
{
    public string DocumentKey { get; set; } = null!;
    public IEnumerable<Email>? Emails { get; set; } = null!;
}