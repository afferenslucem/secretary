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
    
    public LogTimeReminder _reminder = null!;

    [SetUp]
    public void Setup()
    {
        _userStorage = new Mock<IUserStorage>();
        _telegramClient = new Mock<ITelegramClient>();
        
        _reminder = new LogTimeReminder(
            _userStorage.Object, 
            _telegramClient.Object
        );

        _reminder.ShouldSkipDelay = true;
    }

    [Test]
    public void NextDateShouldReturn15For1()
    {
        var result = _reminder.GetNextNotifyDate(new DateTime(2022, 7, 1));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 7, 15)));
    }

    [Test]
    public void NextDateShouldReturn15For15()
    {
        var result = _reminder.GetNextNotifyDate(new DateTime(2022, 7, 15));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 7, 15)));
    }

    [Test]
    public void NextDateShouldReturn31For16July()
    {
        var result = _reminder.GetNextNotifyDate(new DateTime(2022, 7, 16));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 7, 31)));
    }

    [Test]
    public void NextDateShouldReturn28For16February2022()
    {
        var result = _reminder.GetNextNotifyDate(new DateTime(2022, 2, 16));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 2, 28)));
    }

    [Test]
    public void NextDateShouldReturn28For16February2024()
    {
        var result = _reminder.GetNextNotifyDate(new DateTime(2024, 2, 16));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2024, 2, 29)));
    }

    [Test]
    public void ShouldReturnTrueForNextDate()
    {
        var nextDate = new DateOnly(2022, 9, 1);
        var lastDate = new DateOnly(2022, 6, 1);
        var now = new DateTime(2022, 9, 1, 8, 32, 0);
        
        var reader = new Mock<ICalendarReader>();
        var calendar = new Mock<Calendar>();

        reader.Setup(target => target.Read(It.IsAny<int>())).Returns(calendar.Object);
        calendar.Setup(target => target.IsLastWorkingDayBefore(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
            .Returns(true);
        
        _reminder.CalendarReader = reader.Object;

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

        var reader = new Mock<ICalendarReader>();
        var calendar = new Mock<Calendar>();

        reader.Setup(target => target.Read(It.IsAny<int>())).Returns(calendar.Object);
        calendar.Setup(target => target.IsLastWorkingDayBefore(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
            .Returns(true);
        
        _reminder.CalendarReader = reader.Object;

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