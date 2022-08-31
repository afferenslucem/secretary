using Moq;
using secretary.documents.creators;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.timeoff;

namespace secretary.telegram.tests.commands.subcommands.timeoff;

public class CheckEmailsCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IDocumentStorage> _documentStorage = null!;
    private Mock<IEmailStorage> _emailStorage = null!;
    
    private TimeOffCommand _parent = null!;
    private CheckEmailsCommand _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        this._documentStorage = new Mock<IDocumentStorage>();
        this._emailStorage = new Mock<IEmailStorage>();
        this._client = new Mock<ITelegramClient>();

        this._command = new CheckEmailsCommand();

        this._parent = new TimeOffCommand();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            DocumentStorage = this._documentStorage.Object,
            EmailStorage = this._emailStorage.Object,
        };
        
        this._command.Context = _context;
        this._command.ParentCommand = _parent;
    }
    
    [Test]
    public async Task ShouldReturnEmailTable()
    {
        _documentStorage
            .Setup(target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document() { Id = 0 });
        
        _emailStorage
            .Setup(target => target.GetForDocument(It.IsAny<long>()))
            .ReturnsAsync(new []
            {
                new Email() { Address = "a.pushkin@infinnity.ru", DisplayName = "Александр Пушкин"},
                new Email() { Address = "s.esenin@infinnity.ru", DisplayName = "Сергей Есенин"},
                new Email() { Address = "v.mayakovskii@infinnity.ru"},
            });

        await _command.Execute();
        
        _client.Verify(target => target.SendMessageWithKeyBoard(2517, 
            "Заявление будет отправлено на следующие адреса:\r\n" +
            "<code>\r\n" +
            "a.pushkin@infinnity.ru (Александр Пушкин)\r\n" +
            "s.esenin@infinnity.ru (Сергей Есенин)\r\n" +
            "v.mayakovskii@infinnity.ru" +
            "</code>\r\n" +
            "\r\n" +
            "Все верно?",
            new [] { "Верно", "Нет, нужно поправить" }));
    }
    
    [Test]
    public async Task ShouldReturnRunNextOnCorrect()
    {
        _context.Message = "верно";
        var result = await _command.OnMessage();
        
        Assert.That(result, Is.EqualTo(1));
    }
    
    [Test]
    public async Task ShouldReturnRunPrevOnIncorrect()
    {
        _context.Message = "не верно";
        var result = await _command.OnMessage();
        
        Assert.That(result, Is.EqualTo(-1));
    }
}