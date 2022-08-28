using Moq;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.registeruser;

namespace secretary.telegram.tests.commands.regusteruser;

public class EnterNameCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    
    private EnterNameCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._command = new EnterNameCommand();
        
        this._userStorage = new Mock<IUserStorage>();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            UserStorage = this._userStorage.Object,
        };
    }
    
    [Test]
    public async Task ShouldSendExampleMessage()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));

        await this._command.Execute(_context);
        
        this._client.Verify(target => target.SendMessage(2517, "Введите ваши имя и фамилию в именительном падеже.\r\n" +
                                                              "Так они будут указаны в почтовом ящике, с которого будет отправляться письмо.\r\n" +
                                                              @"Например: <i>Александр Пушкин</i>"));
    }
    
    [Test]
    public async Task ShouldSetName()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);

        _context.Message = "Александр Пушкин";
        
        await this._command.OnMessage(_context);
        
        this._userStorage.Verify(target => target.SetUser(
            It.Is<User>(user => user.ChatId == 2517 && user.Name == "Александр Пушкин")
        ));
    }

    [Test]
    public async Task ShouldUpdateName()
    {
        var oldUser = new User
        {
            ChatId = 2517,
            Email = "a.pushkin@infinnity.ru",
        };
        
        _userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);

        _context.Message = "Александр Пушкин";
        
        await this._command.OnMessage(_context);
        
        this._userStorage.Verify(target => target.SetUser(
            It.Is<User>(user => user.ChatId == 2517 && user.Name == "Александр Пушкин" && user.Email == "a.pushkin@infinnity.ru")
        ));
    }
    
}