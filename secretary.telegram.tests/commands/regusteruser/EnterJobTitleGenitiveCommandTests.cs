using Moq;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.registeruser;

namespace secretary.telegram.tests.commands.regusteruser;

public class EnterJobTitleGenitiveCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    
    private EnterJobTitleGenitiveCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._command = new EnterJobTitleGenitiveCommand();
        
        this._userStorage = new Mock<IUserStorage>();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            UserStorage = this._userStorage.Object,
        };
        
        this._command.Context = _context;
    }
    
    [Test]
    public async Task ShouldSendJobTitleGenitiveCommand()
    {
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, "Введите вашу должность в родительном падеже.\r\n" +
                                                              "Так она будут указана в графе \"от кого\".\r\n" +
                                                              @"Например: От <i>поэта</i> Пушкина Александра Сергеевича"));
    }
    
    [Test]
    public async Task ShouldSetJobTitleGenitive()
    {
        var oldUser = new User
        {
            ChatId = 2517,
            Name = "Александр Пушкин",
            NameGenitive = "Пушкина Александра Сергеевича",
            JobTitle = "поэт",
        };
        
        _userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);

        _context.Message = "поэта";
        
        await this._command.OnMessage();
        
        this._userStorage.Verify(target => target.UpdateUser(
            It.Is<User>(user => user.ChatId == 2517 && user.JobTitleGenitive == "поэта" && user.JobTitle == "поэт" && user.NameGenitive == "Пушкина Александра Сергеевича" && user.Name == "Александр Пушкин")
        ));
        
        this._client.Verify(target => target.SendMessage(2517, "Ваш пользователь успешно сохранен"));
    }
}