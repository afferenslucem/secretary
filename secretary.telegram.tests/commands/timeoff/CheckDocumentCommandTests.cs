using Moq;
using secretary.documents.creators;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.timeoff;

namespace secretary.telegram.tests.commands.subcommands.timeoff;

public class CheckDocumentCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ITimeOffCreator> _creator = null!;
    
    private TimeOffCommand _parent = null!;
    private CheckDocumentCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        this._userStorage = new Mock<IUserStorage>();
        this._client = new Mock<ITelegramClient>();
        this._creator = new Mock<ITimeOffCreator>();

        this._command = new CheckDocumentCommand();
        this._command.Creator = _creator.Object;

        this._parent = new TimeOffCommand();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            UserStorage = this._userStorage.Object,
        };
    }
    
    [Test]
    public async Task ShouldSendCheckDocumentCommand()
    {
        _parent.Data.Period = "08.12.2022 с 9:00 до 13:00";
        
        _client.Setup(target => target.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        _client.Setup(target => target.SendDocument(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()));
        _client.Setup(target => target.SendMessageWithKeyBoard(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string[]>()));
        _userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User() { Name = "Александр Пушкин" } );
        _creator.Setup(target => target.Create(It.IsAny<TimeOffData>())).Returns("timeoff-path.docx");

        _context.Message = "Да";
        await this._command.Execute(_context, this._parent);
        
        this._client.Verify(target => target.SendMessage(2517, "Проверьте документ"));
        this._client.Verify(target => target.SendDocument(2517, "timeoff-path.docx", "Александр-Пушкин-08.12.2022-Отгул.docx"));
        this._client.Verify(target => target.SendMessageWithKeyBoard(2517, "Отправить заявление?", new [] {"Да", "Нет"}));
    }
}