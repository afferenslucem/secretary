using Secretary.Storage.Models;

namespace Secretary.Storage.Interfaces;

public interface IEventLogStorage
{
    Task Save(EventLog @event);
}