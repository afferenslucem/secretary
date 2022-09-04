using Moq;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.timeoff;
using secretary.telegram.exceptions;

namespace secretary.telegram.tests.commands.subcommands.timeoff;

public class EnterPeriodCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    
    private TimeOffCommand _parent = null!;
    private EnterPeriodCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();
        this._userStorage = new Mock<IUserStorage>();

        this._command = new EnterPeriodCommand();

        this._parent = new TimeOffCommand();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            UserStorage = _userStorage.Object,
        };
        
        this._command.Context = _context;
        this._command.ParentCommand = _parent;

        this._userStorage.Setup(target => target.GetUser(It.IsAny<long>()))
            .ReturnsAsync(new User() { JobTitleGenitive = "", AccessToken = "" });
    }
    
    [Test]
    public async Task ShouldSendEnterPeriodCommand()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, "Введите период отгула в формате <strong>DD.MM.YYYY[ с HH:mm до HH:mm]</strong>\r\n" +
                                                              "Например: <i>26.04.2020 c 9:00 до 13:00</i>\r\n" +
                                                              "Или: <i>26.04.2020</i>, если вы берете отгул на целый день\r\n" +
                                                              "В таком виде это будет вставлено в документ.\r\n\r\n" +
                                                              "Лучше соблюдать форматы даты и всемени, потому что со временем я хочу еще сделать создание события в календаре яндекса:)"));
    }
    
    [Test]
    public async Task ShouldSetPeriodToCommand()
    {
        _context.Message = "16.08.2022 c 13:00 до 17:00";
      
        await this._command.OnMessage();
        
        Assert.That(_context.Message, Is.EqualTo(_parent.Data.Period));
    }
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredUser()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);

        Assert.ThrowsAsync<NonCompleteUserException>(() => this._command.Execute());
    }
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredMail()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { JobTitleGenitive = "" });

        Assert.ThrowsAsync<NonCompleteUserException>(() => this._command.Execute());
    }
    
    [Test]
    public void ShouldThrowExceptionForUnregisteredPersonalInfo()
    {
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { AccessToken = "" });

        Assert.ThrowsAsync<NonCompleteUserException>(() => this._command.Execute());
    }
}