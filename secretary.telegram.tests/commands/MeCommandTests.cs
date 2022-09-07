using Moq;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands;

public class MeCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;

    private MeCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();
        this._userStorage = new Mock<IUserStorage>();
        this._sessionStorage = new Mock<ISessionStorage>();

        this._command = new MeCommand();

        this._context = new CommandContext()
        {
            ChatId = 2517,
            TelegramClient = this._client.Object,
            UserStorage = _userStorage.Object,
            SessionStorage = _sessionStorage.Object,
        };

        this._command.Context = _context;
    }

    [Test]
    public void ShouldHaveCorrectKey()
    {
        Assert.That(MeCommand.Key, Is.EqualTo("/me"));
    }

    [Test]
    public void ShouldCreate()
    {
        Assert.Pass();
    }

    [Test]
    public async Task ShouldReturnUnregisteredUser()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);

        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, 
                "Вы незарегистрированный пользователь\r\n\r\n" +
                "Для корректной работы вам необходимо выполнить следующие команды:\r\n" +
                "/registeruser\r\n" +
                "/registermail"
            )
        );
    }

    [Test]
    public async Task ShouldReturnEmptyMailUser()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User
        {
            Name = "Александр Пушкин",
            NameGenitive = "Пушкина Александра Сергеевича",
            JobTitle = "поэт",
            JobTitleGenitive = "поэта",
        });

        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, 
                "<strong>Имя:</strong> Александр Пушкин\r\n" +
                "<strong>Имя в Р.П.:</strong> Пушкина Александра Сергеевича\r\n" +
                "<strong>Должность:</strong> поэт\r\n" +
                "<strong>Должность в Р.П.:</strong> поэта\r\n" +
                "<strong>Почта:</strong> не задана\r\n\r\n" +
                "У вас нет токена для почты. Выполните команду /registermail"));
    }

    [Test]
    public async Task ShouldReturnEmptyNameUser()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User
        {
            Email = "a.pushkin@infinnity.ru",
            AccessToken = "token",
        });

        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, 
            "<strong>Имя:</strong> не задано\r\n" +
            "<strong>Имя в Р.П.:</strong> не задано\r\n" +
            "<strong>Должность:</strong> не задана\r\n" +
            "<strong>Должность в Р.П.:</strong> не задана\r\n" +
            "<strong>Почта:</strong> a.pushkin@infinnity.ru\r\n\r\n" +
            "У вас не заданы данные о пользователе. Выполните команду /registeruser"));
    }

    [Test]
    public async Task ShouldReturnFullUser()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User
        {
            Name = "Александр Пушкин",
            NameGenitive = "Пушкина Александра Сергеевича",
            JobTitle = "поэт",
            JobTitleGenitive = "поэта",
            Email = "a.pushkin@infinnity.ru",
            AccessToken = "token",
        });

        await _command.Execute();
        
        _client.Verify(target => target.SendMessage(2517, 
            "<strong>Имя:</strong> Александр Пушкин\r\n" +
            "<strong>Имя в Р.П.:</strong> Пушкина Александра Сергеевича\r\n" +
            "<strong>Должность:</strong> поэт\r\n" +
            "<strong>Должность в Р.П.:</strong> поэта\r\n" +
            "<strong>Почта:</strong> a.pushkin@infinnity.ru"));
    }

    [Test]
    public async Task ShouldDeleteSession()
    {
        await _command.OnComplete();
        
        _sessionStorage.Verify(target => target.DeleteSession(2517), Times.Once);
    }
}