using Moq;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Sessions;

namespace Secretary.Telegram.Tests.Commands;

public class RemindLogTimeTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;

    private RemindLogTimeCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();

        _command = new RemindLogTimeCommand();

        _context = new CommandContext()
        {
            ChatId = 2517,
            TelegramClient = _client.Object,
            UserStorage = _userStorage.Object,
        };

        _command.Context = _context;
    }

    [Test]
    public void ShouldHaveCorrectKey()
    {
        Assert.That(RemindLogTimeCommand.Key, Is.EqualTo("/remindlogtime"));
    }

    [Test]
    public async Task ShouldTurnOnForUnregUser()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);

        await _command.Execute();
        
        _userStorage.Verify(target => target.SetUser(It.Is<User>(target => target.RemindLogTime == true)));
    }

    [Test]
    public async Task ShouldTurnOnForExistingUser()
    {
        _userStorage
            .Setup(target => target.GetUser(It.IsAny<long>()))
            .ReturnsAsync(new User() { RemindLogTime = false } );

        await _command.Execute();
        
        _userStorage.Verify(target => target.SetUser(It.Is<User>(target => target.RemindLogTime == true)));
    }

    [Test]
    public async Task ShouldTurnOffForExistingUser()
    {
        _userStorage
            .Setup(target => target.GetUser(It.IsAny<long>()))
            .ReturnsAsync(new User() { RemindLogTime = true } );

        await _command.Execute();
        
        _userStorage.Verify(target => target.SetUser(It.Is<User>(target => target.RemindLogTime == false)));
    }

    [Test]
    public async Task ShouldSendTurnOnMessage()
    {
        _userStorage
            .Setup(target => target.GetUser(It.IsAny<long>()))
            .ReturnsAsync(new User() { RemindLogTime = false } );

        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, 
            "Напоминания о логгировании времени включены.\n\n" + 
            "Чтобы их выключить отправьте эту команду еще раз"));
    }

    [Test]
    public async Task ShouldSendTurnOffMessage()
    {
        _userStorage
            .Setup(target => target.GetUser(It.IsAny<long>()))
            .ReturnsAsync(new User() { RemindLogTime = true } );

        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, 
            "Напоминания о логгировании времени выключены.\n\n" + 
            "Чтобы их включить отправьте эту команду еще раз"));
    }
}