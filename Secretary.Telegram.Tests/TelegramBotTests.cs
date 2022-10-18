using Moq;
using Secretary.Configuration;
using Secretary.Storage;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.chains;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Sessions;

namespace Secretary.Telegram.Tests;

public class TelegramBotTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<CommandsListeningChain> _chain = null!;

    private Mock<Database> _database = null!;
    
    private TelegramBot _bot = null!;

    [SetUp]
    public void Setup()
    {
        _chain = new Mock<CommandsListeningChain>();
        _client = new Mock<ITelegramClient>();
        _userStorage = new Mock<IUserStorage>();
        _sessionStorage = new Mock<ISessionStorage>();

        _database = new Mock<Database>();

        _database.SetupGet(target => target.UserStorage).Returns(_userStorage.Object);
        
        _bot = new TelegramBot(
            _database.Object, 
            null!, 
            _sessionStorage.Object,
            null!,
            null!,
            _client.Object
        );
        _bot.Chain = _chain.Object;
    }

    [Test]
    public async Task ShouldGetCommandOnMessage()
    {
        _chain.Setup(target => target.Get(It.IsAny<string>())).Returns(new EmptyCommand());
        
        await _bot.WorkWithMessage(new UserMessage(2517, "pushkin", "/command"));
        
        _chain.Verify(target => target.Get("/command"), Times.Once);
    }

    [Test]
    public async Task ShouldExecuteCommandOnMessage()
    {
        var command = new Mock<Command>();
        
        _chain.Setup(
            target => target.Get(It.IsAny<string>())
        ).Returns(command.Object);
        
        await _bot.WorkWithMessage(new UserMessage(2517, "pushkin", "/command"));
        
        command.Verify(target => target.Execute(), Times.Once);
    }

    [Test]
    public async Task ShouldCompleteCommandOnMessage()
    {
        var command = new Mock<Command>();
        
        _chain.Setup(
            target => target.Get(It.IsAny<string>())
        ).Returns(command.Object);
        
        await _bot.WorkWithMessage(new UserMessage(2517, "pushkin", "/command"));
        
        command.Verify(target => target.OnComplete(), Times.Once);
    }

    [Test]
    public async Task ShouldHandleUnregUserException()
    {
        var command = new Mock<Command>();

        command.Setup(
            target => target.Execute()
        ).ThrowsAsync(new NonCompleteUserException("User does not exist"));
        
        _chain.Setup(
            target => target.Get(It.IsAny<string>())
        ).Returns(command.Object);
        
        await _bot.WorkWithMessage(new UserMessage(2517, "pushkin", "/command"));
        
        _client.Verify(target => target.SendMessage(2517,
                "Вы – незарегистрированный пользователь.\n\n" +
                "Выполните команды:\n" +
                "/registeruser\n" +
                "/registermail"), 
            Times.Once);
        
        _sessionStorage.Verify(target => target.DeleteSession(2517));
    }

    [Test]
    public async Task ShouldHandleException()
    {
        var command = new Mock<Command>();

        command.Setup(
            target => target.Execute()
        ).ThrowsAsync(new Exception("Boo"));
        
        _chain.Setup(
            target => target.Get(It.IsAny<string>())
        ).Returns(command.Object);
        
        await _bot.WorkWithMessage(new UserMessage(2517, "pushkin", "/command"));
        
        _client.Verify(target => target.SendMessage(2517,
                "Произошла непредвиденная ошибка"), 
            Times.Once);
        
        _sessionStorage.Verify(target => target.DeleteSession(2517));
    }
}