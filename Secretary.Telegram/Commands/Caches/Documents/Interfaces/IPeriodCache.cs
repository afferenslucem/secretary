using Secretary.Telegram.Models;

namespace Secretary.Telegram.Commands.Caches.Documents.Interfaces;

public interface IPeriodCache
{
    DatePeriod? Period { get; set; }
}