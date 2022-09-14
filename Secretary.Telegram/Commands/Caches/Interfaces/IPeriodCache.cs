using Secretary.Telegram.Models;

namespace Secretary.Telegram.Commands.Caches.Interfaces;

public interface IPeriodCache
{
    DatePeriod? Period { get; set; }
}