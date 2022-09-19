using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands.Distant;
using Secretary.Telegram.Commands.TimeOff;
using Secretary.Telegram.Commands.Vacation;

namespace Secretary.Telegram.Wrappers;

public class StatisticService
{
    private DateTime UtcNow
    {
        get
        {
            return DateTime.UtcNow;;
        }
    }
    
    private IEventLogStorage _eventLogStorage;

    public StatisticService(IEventLogStorage eventLogStorage)
    {
        _eventLogStorage = eventLogStorage;
    }

    public async Task LogTimeOff(long chatId)
    {
        var @event = new EventLog
        {
            Time = UtcNow,
            EventType = TimeOffCommand.Key,
            UserChatId = chatId,
            Description = "Created Time Off"
        };

        await _eventLogStorage.Save(@event);
    }

    public async Task LogDistant(long chatId)
    {
        var @event = new EventLog
        {
            Time = UtcNow,
            EventType = DistantCommand.Key,
            UserChatId = chatId,
            Description = "Created Distant"
        };

        await _eventLogStorage.Save(@event);
    }

    public async Task LogVacation(long chatId)
    {
        var @event = new EventLog
        {
            Time = UtcNow,
            EventType = VacationCommand.Key,
            UserChatId = chatId,
            Description = "Created Vacation"
        };

        await _eventLogStorage.Save(@event);
    }
}