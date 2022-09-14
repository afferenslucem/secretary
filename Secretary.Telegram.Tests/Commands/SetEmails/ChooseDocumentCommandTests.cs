using Moq;
using Secretary.Storage;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.SetEmails;
using Secretary.Telegram.Exceptions;

namespace Secretary.Telegram.Tests.Commands.SetEmails;

public class ChooseDocumentCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    
    private CommandContext _context = null!;
    private ChooseDocumentCommand _command = null!;

    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();

        _context = new CommandContext
        {
            ChatId = 2517,
            TelegramClient = _client.Object,
            UserStorage = _userStorage.Object,
        };

        _command = new ChooseDocumentCommand();

        _command.Context = _context;
        
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>()))
            .ReturnsAsync(new User() { JobTitleGenitive = "", AccessToken = "" });
    }

    [Test]
    public async Task ShouldSendGreeting()
    {
        await _command.Execute();
        
        _client.Verify(target => target.SendMessageWithKeyBoard(2517,
            "Выберете документ для установки получателей",
            new []{ "Отгул", "Отпуск" }
        ));
    }
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredUser()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);

        Assert.ThrowsAsync<NonCompleteUserException>(() => _command.Execute());
    }
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredMail()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { JobTitleGenitive = "" });

        Assert.ThrowsAsync<NonCompleteUserException>(() => _command.Execute());
    }
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredPersonalInfo()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { AccessToken = "" });

        Assert.ThrowsAsync<NonCompleteUserException>(() => _command.Execute());
    }
    
    [Test]
    public void ShouldThrowIncorrectMessageException()
    {
        _context.Message = "Ghbdtn";
        Assert.ThrowsAsync<IncorrectMessageException>(() => _command.OnMessage());
    }
}