using Moq;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Sessions;

namespace Secretary.Telegram.Tests.Commands;

public class StartCommandTests
{
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ITelegramClient> _client = null!;

    private StartCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();

        _command = new StartCommand();
        _sessionStorage = new Mock<ISessionStorage>();
        _userStorage = new Mock<IUserStorage>();

        _context = new CommandContext()
        {
            ChatId = 2517,
            TelegramClient = _client.Object,
            SessionStorage = _sessionStorage.Object,
            UserStorage = _userStorage.Object,
        };

        _command.Context = _context;
    }

    [Test]
    public void ShouldHaveCorrectKey()
    {
        Assert.That(StartCommand.Key, Is.EqualTo("/start"));
    }

    [Test]
    public async Task ShouldSendExampleMessage()
    {
        await _command.Execute();

        _client.Verify(target => target.SendMessage(2517, "Добро пожаловать!\n" +
                                                          "\n" +
                                                          "Перед началом работы вам необходимо:\n" +
                                                          "1. /registeruser – зарегистрироваться\n" +
                                                          "2./registermail – зарегистрировать рабочую почту\n" +
                                                          "3. <a href=\"https://mail.yandex.ru/#setup/client\">Разрешить доступ по протоколу IMAP</a>"
            )
        );
    }

    [Test]
    public async Task ShouldRegisterNewUsers()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);
        
        await _command.Execute();

        _userStorage.Verify(target => target.SetUser(It.Is<User>(user => user.ChatId == 2517)), Times.Once);
    }

    [Test]
    public async Task ShouldSkipRegistersers()
    {
        var oldUser = new User();
        
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);
        
        await _command.Execute();

        _userStorage.Verify(target => target.SetUser(It.Is<User>(user => user.ChatId == 2517)), Times.Never);
    }

    [Test]
    public async Task ShouldDeleteSession()
    {
        await _command.OnComplete();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
    }
}