using Moq;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.WorkingCalendar;
using Secretary.WorkingCalendar.Models;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Exceptions;

namespace Secretary.Telegram.Tests;

public class LogTimeReminderTests
{
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ITelegramClient> _telegramClient = null!;
    
    private Mock<ICalendarReader> _reader;
    private Mock<Calendar> _calendar;
    
    public LogTimeReminder _reminder = null!;

    [SetUp]
    public void Setup()
    {
        _reader = new Mock<ICalendarReader>();
        _calendar = new Mock<Calendar>();
        
        _reader.Setup(target => target.Read(It.IsAny<int>())).Returns(_calendar.Object);
        
        _userStorage = new Mock<IUserStorage>();
        _telegramClient = new Mock<ITelegramClient>();
        
        _reminder = new LogTimeReminder(
            _userStorage.Object, 
            _telegramClient.Object
        );

        _reminder.CalendarReader = _reader.Object;

        _reminder.ShouldSkipDelay = true;
    }

    [Test]
    public void NextDateShouldUse15For1()
    {
        _calendar
            .Setup(
                target => target.GetLastWorkingDayBefore(
                    It.IsAny<DateOnly>(), 
                    It.IsAny<DateOnly>()
                )
            )
            .Returns(new DateOnly(2022, 7, 15));
        
        var result = _reminder.GetNextNotifyDate(new DateTime(2022, 7, 1));
        
        _calendar.Verify(
            target => target.GetLastWorkingDayBefore(new DateOnly(2022, 7, 1), new DateOnly(2022, 7, 15)));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 7, 15)));
    }

    [Test]
    public void NextDateShouldReturn15For15()
    {
        _calendar
            .Setup(
                target => target.GetLastWorkingDayBefore(
                    It.IsAny<DateOnly>(), 
                    It.IsAny<DateOnly>()
                )
            )
            .Returns(new DateOnly(2022, 7, 15));
        
        var result = _reminder.GetNextNotifyDate(new DateTime(2022, 7, 15));
        
        _calendar.Verify(
            target => target.GetLastWorkingDayBefore(new DateOnly(2022, 7, 15), new DateOnly(2022, 7, 15)));

        Assert.That(result, Is.EqualTo(new DateOnly(2022, 7, 15)));
    }

    [Test]
    public void NextDateShouldReturn31For16July()
    {
        _calendar
            .Setup(
                target => target.GetLastWorkingDayBefore(
                    It.IsAny<DateOnly>(), 
                    It.IsAny<DateOnly>()
                )
            )
            .Returns(new DateOnly(2022, 7, 31));
        
        var result = _reminder.GetNextNotifyDate(new DateTime(2022, 7, 16));
        
        _calendar.Verify(
            target => target.GetLastWorkingDayBefore(new DateOnly(2022, 7, 16), new DateOnly(2022, 7, 31)));

        Assert.That(result, Is.EqualTo(new DateOnly(2022, 7, 31)));
    }

    [Test]
    public void NextDateShouldReturn28For16February2022()
    {
        _calendar
            .Setup(
                target => target.GetLastWorkingDayBefore(
                    It.IsAny<DateOnly>(), 
                    It.IsAny<DateOnly>()
                )
            )
            .Returns(new DateOnly(2022, 2, 28));
        
        var result = _reminder.GetNextNotifyDate(new DateTime(2022, 2, 16));
        
        _calendar.Verify(
            target => target.GetLastWorkingDayBefore(new DateOnly(2022, 2, 16), new DateOnly(2022, 2, 28)));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 2, 28)));
    }

    [Test]
    public void NextDateShouldReturn15ForNextDay30September2022()
    {
        _calendar
            .Setup(
                target => target.GetLastWorkingDayBefore(
                    It.IsAny<DateOnly>(), 
                    It.IsAny<DateOnly>()
                )
            )
            .Returns(new DateOnly(2022, 10, 15));

        var now = new DateTime(2022, 9, 30).AddDays(1);
        
        var result = _reminder.GetNextNotifyDate(now);
        
        _calendar.Verify(
            target => target.GetLastWorkingDayBefore(new DateOnly(2022, 10, 1), new DateOnly(2022, 10, 15)));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 10, 15)));
    }

    [Test]
    public void NextDateShouldReturn28For16February2024()
    {
        _calendar
            .Setup(
                target => target.GetLastWorkingDayBefore(
                    It.IsAny<DateOnly>(), 
                    It.IsAny<DateOnly>()
                )
            )
            .Returns(new DateOnly(2024, 2, 29));
        
        var result = _reminder.GetNextNotifyDate(new DateTime(2024, 2, 16));
        
        _calendar.Verify(
            target => target.GetLastWorkingDayBefore(new DateOnly(2024, 2, 16), new DateOnly(2024, 2, 29)));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2024, 2, 29)));
    }

    [Test]
    public void ShouldReturnTrueForNextDate()
    {
        var nextDate = new DateOnly(2022, 9, 1);
        var lastDate = new DateOnly(2022, 6, 1);
        var now = new DateTime(2022, 9, 1, 8, 32, 0);

        var result = _reminder.ItsTimeToNotify(nextDate, lastDate, now);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void ShouldReturnFalseForNextDateEqPrevDate()
    {
        var nextDate = new DateOnly(2022, 9, 1);
        var lastDate = new DateOnly(2022, 9, 1);
        var now = new DateTime(2022, 9, 1, 8, 43, 0);

        var result = _reminder.ItsTimeToNotify(nextDate, lastDate, now);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void ShouldReturnFalseForNowNotEqNextDate()
    {
        var nextDate = new DateOnly(2022, 9, 1);
        var lastDate = new DateOnly(2022, 9, 1);
        var now = new DateTime(2022, 8, 1, 8, 32, 0);

        var result = _reminder.ItsTimeToNotify(nextDate, lastDate, now);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void ShouldReturnFalseForNowNotEightHour()
    {
        var nextDate = new DateOnly(2022, 9, 1);
        var lastDate = new DateOnly(2022, 6, 1);
        var now = new DateTime(2022, 9, 1, 9, 0, 0);

        var result = _reminder.ItsTimeToNotify(nextDate, lastDate, now);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task ShouldSendNotify()
    {
        var user = new User
        {
            ChatId = 2517
        };

        await _reminder.Notify(user);
        
        _telegramClient.Verify(target => target.SendMessage(2517, "Не забудьте залоггировать время!"));
    }

    [Test]
    public async Task ShouldSendNotifyForAllUsers()
    {
        var user = new User
        {
            ChatId = 2517
        };
        var user2 = new User
        {
            ChatId = 2518
        };

        var users = new User[] { user, user2 };

        _userStorage.Setup(target => target.GetUsers(user => user.RemindLogTime)).ReturnsAsync(users);
        
        await _reminder.RefreshTokensForAllUsers();
        
        _telegramClient.Verify(target => target.SendMessage(2517, "Не забудьте залоггировать время!"));
        _telegramClient.Verify(target => target.SendMessage(2518, "Не забудьте залоггировать время!"));
    }
}