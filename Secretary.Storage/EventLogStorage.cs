using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;

namespace Secretary.Storage;

public class EventLogStorage : IEventLogStorage
{
    public async Task Save(EventLog @event)
    {
        await using var context = new DatabaseContext();

        await context.EventLogs.AddAsync(@event);

        await context.SaveChangesAsync();
    }
}