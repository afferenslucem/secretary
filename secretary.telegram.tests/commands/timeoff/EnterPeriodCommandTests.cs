using Moq;
using secretary.telegram.commands;
using secretary.telegram.commands.timeoff;

namespace secretary.telegram.tests.commands.subcommands.timeoff;

public class EnterPeriodCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    
    private TimeOffCommand _parent = null!;
    private EnterPeriodCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._command = new EnterPeriodCommand();

        this._parent = new TimeOffCommand();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
        };
    }
    
    [Test]
    public async Task ShouldSendEnterPeriodCommand()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        
        await this._command.Execute(_context);
        
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
      
        await this._command.OnMessage(_context, _parent);
        
        Assert.That(_context.Message, Is.EqualTo(_parent.Data.Period));
    }
}