using Moq;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.registermail;

namespace secretary.telegram.tests.commands.registermail;

public class EnterEmailCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage= null!;
    private CommandContext _context= null!;
    private EnterEmailCommand _command= null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();
        this._userStorage = new Mock<IUserStorage>();

        this._context = new CommandContext()
        {
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            UserStorage = _userStorage.Object,
        };

        this._command = new EnterEmailCommand();
    }

    [Test]
    public void ShouldCreate()
    {
        Assert.Pass();
    }
    
    [Test]
    public async Task ShouldSendEnterEmail()
    {
        await this._command.Execute(_context);
        
        this._client.Verify(target => target.SendMessage(2517, "Введите вашу почту, с которой вы отправляете заявления.\r\n" +
                                                              @"Например: <i>a.pushkin@infinnity.ru</i>"));
    }
    
            
    [Test]
    public async Task ShouldSetEmail()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);

        _context.Message = "a.pushkin@infinnity.ru";
        
        await this._command.OnMessage(_context);
        
        this._userStorage.Verify(target => target.SetUser(
            It.Is<User>(user => user.ChatId == 2517 && user.Email == "a.pushkin@infinnity.ru")
        ));
    }
        
    [Test]
    public async Task ShouldUpdateEmail()
    {
        var oldUser = new User
        {
            ChatId = 2517,
            Name = "Александр Пушкин",
        };
        
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);

        _context.Message = "a.pushkin@infinnity.ru";
        
        await this._command.OnMessage(_context);
        
        this._userStorage.Verify(target => target.SetUser(
            It.Is<User>(user => user.ChatId == 2517 && user.Name == "Александр Пушкин" && user.Email == "a.pushkin@infinnity.ru")
        ));
    }
}