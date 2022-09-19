using Microsoft.EntityFrameworkCore;
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

    public async Task<(string DocumentName, int Count)[]> GetDocumentStatistic(params string[] documentTypes)
    {
        await using var context = new DatabaseContext();

        var data = await context.EventLogs
            .Where(item => documentTypes.Contains(item.EventType))
            .GroupBy(item => item.EventType)
            .Select(grouping => new { DocumentName = grouping.Key, Count = grouping.Count() })
            .ToArrayAsync();

        return data.Select(item => (item.DocumentName, item.Count)).ToArray();
    }
}