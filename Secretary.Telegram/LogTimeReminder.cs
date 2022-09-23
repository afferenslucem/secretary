using Secretary.HealthCheck.Data;
using Secretary.Logging;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.WorkingCalendar;
using Serilog;

namespace Secretary.Telegram;

public class LogTimeReminder
{
    private readonly ILogger _logger = LogPoint.GetLogger<LogTimeReminder>();
    
    private readonly IUserStorage _userStorage;
    private readonly ITelegramClient _telegramClient;
    private readonly CancellationTokenSource _cancellationTokenSource;
    
    public DateTime LastDateCheck { get; set; }
    
    public DateOnly LastNotifyDate { get; set; }
    public DateOnly NextNotifyDate { get; set; }

    public ICalendarReader CalendarReader;

    public bool ShouldSkipDelay { get; set; } = false;
    
    public LogTimeReminder(
        IUserStorage userStorage,
        ITelegramClient telegramClient
    )
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _userStorage = userStorage;
        _telegramClient = telegramClient;
        CalendarReader = new CalendarReader();
    }

    public async Task RunThread()
    {
        _logger.Information("Run refresh token thread");

        NextNotifyDate = GetNextNotifyDate(DateTime.UtcNow);
        
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            await this.Notify();
        }
    }

    public async Task Notify()
    {
        try
        {
            var now = DateTime.UtcNow;

            LastDateCheck = now;
            
            if (ItsTimeToNotify(NextNotifyDate, LastNotifyDate, now))
            {
                _logger.Information("Run token refreshing");
                
                await RefreshTokensForAllUsers();

                LastNotifyDate = NextNotifyDate;
                NextNotifyDate = GetNextNotifyDate(now.AddDays(1));
            }
            
            await Sleep(TimeSpan.FromMinutes(1), _cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Could not refresh tokens");
            await Sleep(TimeSpan.FromMinutes(5), _cancellationTokenSource.Token);
        }
    }
    
    public async Task RefreshTokensForAllUsers()
    {
        var users = await _userStorage.GetUsers(user => user.RemindLogTime);

        foreach (var user in users)
        {
            await Notify(user);
        }
    }

    public async Task Notify(User user)
    {
        try
        {
            _logger.Debug($"send notification for user {user.ChatId}");

            await _telegramClient.SendMessage(user.ChatId, "Не забудьте залоггировать время!");

            _logger.Debug($"Notified user {user.ChatId}");
        }
        catch (Exception e)
        {
            _logger.Warning(e, $"Could not notify user {user.ChatId}");
        }
    }
    
    public bool ItsTimeToNotify(DateOnly nextDate, DateOnly prevDate, DateTime now)
    {
        if (nextDate == prevDate) return false;
        
        if (nextDate == DateOnly.FromDateTime(now) && now.Hour == 8)
        {
            return true;
        }

        return false;
    }

    public DateOnly GetNextNotifyDate(DateTime now)
    {
        DateOnly temp;
        
        if (now.Day > 15)
        {
            var lastDay = DateTime.DaysInMonth(now.Year, now.Month);

            temp = new DateOnly(now.Year, now.Month, lastDay);
        }
        else
        {
            temp = new DateOnly(now.Year, now.Month, 15);
        }
        
        var calendar = CalendarReader.Read(now.Year);

        var result = calendar.GetLastWorkingDayBefore(DateOnly.FromDateTime(now), temp);
        
        _logger.Information($"Next date to refresh {result}");

        return result;
    }
    
    public Task Sleep(TimeSpan span, CancellationToken cancellationToken)
    {
        if (ShouldSkipDelay)
        {
            return Task.CompletedTask;;
        }
        
        return Task.Delay(span, cancellationToken);
    }

    public ReminderHealthData GetHealthData()
    {
        var result = new ReminderHealthData();

        result.PingTime = LastDateCheck;
        result.NextNotifyDate = NextNotifyDate.ToDateTime(TimeOnly.MinValue);

        return result;
    }
}