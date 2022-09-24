using Moq;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram;

namespace Secretary.LogTimeReminder.Tests;

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
    public void ShouldReturn4MarchLastWorkingDay2022()
    {
        var result = _reminder.GetLastWorkingDayBefore(new DateTime(2022, 3, 4), new DateOnly(2022, 3, 6));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 3, 5)));
    }

    [Test]
    public void ShouldReturn13MayLastWorkingDay2022()
    {
        var result = _reminder.GetLastWorkingDayBefore(new DateTime(2022, 5, 11), new DateOnly(2022, 5, 15));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 5, 13)));
    }

    [Test]
    public void ShouldReturn15AugustLastWorkingDay2022()
    {
        var result = _reminder.GetLastWorkingDayBefore(new DateTime(2022, 8, 10), new DateOnly(2022, 8, 15));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 8, 15)));
    }
    
    [Test]
    public void NextDateShouldUse13For15May2022()
    {
        var result = _reminder.GetNextNotifyDate(new DateTime(2022, 5, 11));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 5, 13)));
    }
    
    [Test]
    public void NextDateShouldUse30For16September2022()
    {
        var result = _reminder.GetNextNotifyDate(new DateTime(2022, 9, 16));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 9, 30)));
    }
    
    [Test]
    public void NextDateShouldUse28For20February2022()
    {
        var result = _reminder.GetNextNotifyDate(new DateTime(2022, 2, 20));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 2, 28)));
    }

    [Test]
    public void NextDateShouldUse15For10August2022()
    {
        var result = _reminder.GetNextNotifyDate(new DateTime(2022, 8, 10));
        
        Assert.That(result, Is.EqualTo(new DateOnly(2022, 8, 15)));
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
        
        await _reminder.NotifyAllUsers();
        
        _telegramClient.Verify(target => target.SendMessage(2517, "Не забудьте залоггировать время!"));
        _telegramClient.Verify(target => target.SendMessage(2518, "Не забудьте залоггировать время!"));
    }
}