using Moq;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.registeruser;

namespace secretary.telegram.tests.commands.regusteruser;

public class EnterNameGenitiveCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    
    private EnterNameGenitiveCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._command = new EnterNameGenitiveCommand();
        
        this._userStorage = new Mock<IUserStorage>();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            UserStorage = this._userStorage.Object,
        };
    }
    
    [Test]
    public async Task ShouldSendExample()
    {
        await this._command.Execute(_context);
        
        this._client.Verify(target => target.SendMessage(2517, "Введите ваши имя и фамилию в родительном падеже.\r\n" +
                                                               "Так они будут указаны в отправоляемом документе в графе \"от кого\".\r\n" +
                                                               @"Например: От <i>Пушкина Александра Сергеевича</i>"));
    }
    
    [Test]
    public async Task ShouldSetNameGenitive()
    {
        var oldUser = new User
        {
            ChatId = 2517,
            Name = "Александр Пушкин",
        };
        
        _userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);

        _context.Message = "Пушкина Александра Сергеевича";
        
        await this._command.OnMessage(_context);
        
        this._userStorage.Verify(target => target.UpdateUser(
            It.Is<User>(user => user.ChatId == 2517 && user.NameGenitive == "Пушкина Александра Сергеевича" && user.Name == "Александр Пушкин")
        ));
    }
}