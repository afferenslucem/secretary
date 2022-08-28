using Moq;
using secretary.telegram.commands;
using secretary.telegram.sessions;
using secretary.storage;
using secretary.storage.models;

namespace secretary.telegram.tests.commands;

public class RegisterUserCommandTests
{
    private Mock<ITelegramClient> client;
    private Mock<IUserStorage> userStorage;
    private Mock<ISessionStorage> sessionStorage;
    private CommandContext context;
    private RegisterUserCommand command;
        
    [SetUp]
    public void Setup()
    {
        this.client = new Mock<ITelegramClient>();
        this.userStorage = new Mock<IUserStorage>();
        this.sessionStorage = new Mock<ISessionStorage>();

        this.context = new CommandContext()
        {
            ChatId = 2517, 
            TelegramClient = this.client.Object, 
            SessionStorage = sessionStorage.Object, 
            UserStorage = userStorage.Object,
        };

        this.command = new RegisterUserCommand();
    }
        
    [Test]
    public async Task ShouldSendNameCommand()
    {
        client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));

        await this.command.Execute(context);
        
        this.client.Verify(target => target.SendMessage(2517, "Введите ваши имя и фамилию в именительном падеже.\r\n" +
                                                              "Так они будут указаны в почтовом ящике, с которого будет отправляться письмо.\r\n" +
                                                              @"Например: <i>Александр Пушкин</i>"));
    }
        
    [Test]
    public async Task ShouldSetName()
    {
        await SkipStep();
        
        client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);
        userStorage.Setup(target => target.SetUser(It.IsAny<User>()));

        context.Message = "Александр Пушкин";
        
        await this.command.Execute(context);
        
        this.userStorage.Verify(target => target.SetUser(
            It.Is<User>(user => user.ChatId == 2517 && user.Name == "Александр Пушкин")
        ));
    }
        
    [Test]
    public async Task ShouldUpdateName()
    {
        await SkipStep();
        
        var oldUser = new User
        {
            ChatId = 2517,
            Email = "a.pushkin@infinnity.ru",
        };
        
        client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);
        userStorage.Setup(obj => obj.SetUser(It.IsAny<User>()));

        context.Message = "Александр Пушкин";
        
        await this.command.Execute(context);
        
        this.userStorage.Verify(target => target.SetUser(
            It.Is<User>(user => user.ChatId == 2517 && user.Name == "Александр Пушкин" && user.Email == "a.pushkin@infinnity.ru")
        ));
    }
        
    [Test]
    public async Task ShouldSendNameGenitiveCommand()
    {
        await SkipStep();
        
        client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));

        await this.command.Execute(context);
        
        this.client.Verify(target => target.SendMessage(2517, "Введите ваши имя и фамилию в родительном падеже.\r\n" +
                                                              "Так они будут указаны в отправоляемом документе в графе \"от кого\".\r\n" +
                                                              @"Например: От <i>Пушкина Александра Сергеевича</i>"));
    }
    
    [Test]
    public async Task ShouldSetNameGenitive()
    {
        await SkipStep();
        await SkipStep();
        
        var oldUser = new User
        {
            ChatId = 2517,
            Name = "Александр Пушкин",
        };
        
        client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);
        userStorage.Setup(obj => obj.UpdateUser(It.IsAny<User>()));

        context.Message = "Пушкина Александра Сергеевича";
        
        await this.command.Execute(context);
        
        this.userStorage.Verify(target => target.UpdateUser(
            It.Is<User>(user => user.ChatId == 2517 && user.NameGenitive == "Пушкина Александра Сергеевича" && user.Name == "Александр Пушкин")
        ));
    }
        
    [Test]
    public async Task ShouldSendJobTitleCommand()
    {
        userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(new User());
        
        await SkipStep();
        await SkipStep();
        
        client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));

        await this.command.Execute(context);
        
        this.client.Verify(target => target.SendMessage(2517, "Введите вашу должность в именительном падеже.\r\n" +
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
        
        userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(new User());
        
        await SkipStep();
        await SkipStep();
        await SkipStep();
        
        client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        userStorage.Setup(obj => obj.UpdateUser(It.IsAny<User>()));
        userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);

        context.Message = "поэт";
        
        await this.command.Execute(context);
        
        this.userStorage.Verify(target => target.UpdateUser(
            It.Is<User>(user => user.ChatId == 2517 && user.JobTitle == "поэт" && user.NameGenitive == "Пушкина Александра Сергеевича" && user.Name == "Александр Пушкин")
        ));
    }
    
    [Test]
    public async Task ShouldSendJobTitleGenitiveCommand()
    {
        userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(new User());
        
        await SkipStep();
        await SkipStep();
        await SkipStep();
        
        client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));

        await this.command.Execute(context);
        
        this.client.Verify(target => target.SendMessage(2517, "Введите вашу должность в родительном падеже.\r\n" +
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
        
        userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(new User());
        
        await SkipStep();
        await SkipStep();
        await SkipStep();
        await SkipStep();
        
        client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        userStorage.Setup(obj => obj.UpdateUser(It.IsAny<User>()));
        userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);

        context.Message = "поэта";
        
        await this.command.Execute(context);
        
        this.userStorage.Verify(target => target.UpdateUser(
            It.Is<User>(user => user.ChatId == 2517 && user.JobTitleGenitive == "поэта" && user.JobTitle == "поэт" && user.NameGenitive == "Пушкина Александра Сергеевича" && user.Name == "Александр Пушкин")
        ));
        
        this.client.Verify(target => target.SendMessage(2517, "Ваш пользователь успешно сохранен"));
    }
    
    private Task SkipStep()
    {
        return this.command.Execute(context);
    }
}