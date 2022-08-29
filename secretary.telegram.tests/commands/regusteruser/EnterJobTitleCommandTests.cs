using Moq;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.registeruser;

namespace secretary.telegram.tests.commands.regusteruser;

public class EnterJobTitleCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    
    private EnterJobTitleCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._command = new EnterJobTitleCommand();
        
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
        await this._command.Execute(_context);
        
        this._client.Verify(target => target.SendMessage(2517, "Введите вашу должность в именительном падеже.\r\n" +
                                                               "Так она будут указана в подписи письма.\r\n" +
                                                               @"Например: С уважением, <i>поэт</i> Александр Пушкин"));
    }
    
    [Test]
    public async Task ShouldSetJobTitle()
    {
        var oldUser = new User
        {
            ChatId = 2517,
            Name = "Александр Пушкин",
            NameGenitive = "Пушкина Александра Сергеевича",
        };
        
        _userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);

        _context.Message = "поэт";
        
        await this._command.OnMessage(_context);
        
        this._userStorage.Verify(target => target.UpdateUser(
            It.Is<User>(user => user.ChatId == 2517 && user.JobTitle == "поэт" && user.NameGenitive == "Пушкина Александра Сергеевича" && user.Name == "Александр Пушкин")
        ));
    }
}