using Moq;
using secretary.configuration;
using secretary.documents;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.timeoff;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands;

public class TimeOffCommandTests
{
    private Mock<ITelegramClient> client;
    private Mock<ISessionStorage> sessionStorage;
    private Mock<IUserStorage> userStorage;
    
    private TimeOffCommand command;
    private CommandContext context;
        
    [SetUp]
    public void Setup()
    {
        this.client = new Mock<ITelegramClient>();

        this.sessionStorage = new Mock<ISessionStorage>();

        this.userStorage = new Mock<IUserStorage>();

        this.command = new TimeOffCommand();
        
        this.context = new CommandContext()
            { 
                ChatId = 2517, 
                TelegramClient = this.client.Object, 
                SessionStorage = sessionStorage.Object, 
                UserStorage = userStorage.Object
            };
    }
    
    [Test]
    public async Task ShouldSaveSessionOnExecute()
    {
        sessionStorage.Setup(obj => obj.SaveSession(It.IsAny<long>(), It.IsAny<Session>()));

        await this.command.Execute(context);
        
        this.sessionStorage.Verify(target => target.SaveSession(2517, It.Is<Session>(session => session.ChaitId == 2517 && session.LastCommand == command)));
    }
}