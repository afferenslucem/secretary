using Moq;
using secretary.mail;
using secretary.mail.Authentication;
using secretary.telegram.commands;
using secretary.telegram.sessions;
using secretary.storage;
using secretary.storage.models;

namespace secretary.telegram.tests.commands;

public class RegisterMailCommandTests
{
    private Mock<ITelegramClient> client;
    private Mock<IUserStorage> userStorage;
    private Mock<IYandexAuthenticator> mailClient;
    private Mock<ISessionStorage> sessionStorage;
    private CommandContext context;
    private RegisterMailCommand command;
        
    [SetUp]
    public void Setup()
    {
        this.client = new Mock<ITelegramClient>();
        this.userStorage = new Mock<IUserStorage>();
        this.sessionStorage = new Mock<ISessionStorage>();
        this.mailClient = new Mock<IYandexAuthenticator>();

        this.context = new CommandContext()
        {
            ChatId = 2517, 
            TelegramClient = this.client.Object, 
            YandexAuthenticator = mailClient.Object, 
            UserStorage = userStorage.Object,
            SessionStorage = sessionStorage.Object, 
        };

        this.command = new RegisterMailCommand();
    }
        
    [Test]
    public async Task ShouldSendEnterEmail()
    {
        client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));

        await this.command.Execute(context);
        
        this.client.Verify(target => target.SendMessage(2517, "Введите вашу почту, с которой вы отправляете заявления.\r\n" +
                                                              @"Например: <i>a.pushkin@infinnity.ru</i>"));
    }
    
            
    [Test]
    public async Task ShouldSetEmail()
    {
        await SkipStep();
        
        client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync((User?)null);
        userStorage.Setup(target => target.SetUser(It.IsAny<User>()));
        mailClient.Setup(target => target.GetAuthenticationCode()).ReturnsAsync(new AuthenticationData() {user_code = "", verification_url = ""});

        context.Message = "a.pushkin@infinnity.ru";
        
        await this.command.Execute(context);
        
        this.userStorage.Verify(target => target.SetUser(
            It.Is<User>(user => user.ChatId == 2517 && user.Email == "a.pushkin@infinnity.ru")
        ));
    }
        
    [Test]
    public async Task ShouldUpdateEmail()
    {
        await SkipStep();
        
        var oldUser = new User
        {
            ChatId = 2517,
            Name = "Александр Пушкин",
        };
        
        client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);
        userStorage.Setup(target => target.SetUser(It.IsAny<User>()));
        mailClient.Setup(target => target.GetAuthenticationCode()).ReturnsAsync(new AuthenticationData() {user_code = "", verification_url = ""});

        context.Message = "a.pushkin@infinnity.ru";
        
        await this.command.Execute(context);
        
        this.userStorage.Verify(target => target.SetUser(
            It.Is<User>(user => user.ChatId == 2517 && user.Name == "Александр Пушкин" && user.Email == "a.pushkin@infinnity.ru")
        ));
    }
    
    [Test]
    public async Task ShouldSendCode()
    {
        await SkipStep();
        
        mailClient.Setup(target => target.GetAuthenticationCode()).ReturnsAsync(
            new AuthenticationData()
            {
                user_code = "code",
                verification_url = "url",
            }
        );

        mailClient.Setup(target => target.CheckToken(It.IsAny<AuthenticationData>()))
            .ReturnsAsync(new TokenData() { access_token = "token"});
        
        client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));

        await this.command.Execute(context);
        
        this.client.Verify(target => target.SendMessage(2517, "Пожалуйста, <strong>УБЕДИТЕСЬ</strong>, что вы авторизуетесь в рабочей почте!\r\n" +
                                                              "Введите этот код: <code>code</code> в поле ввода по этой ссылке: url"));
    }
    
        
    [Test]
    public async Task ShouldSendDone()
    {
        await SkipStep();
        
        mailClient.Setup(target => target.GetAuthenticationCode()).ReturnsAsync(
            new AuthenticationData()
            {
                user_code = "code",
                verification_url = "url",
            }
        );
        userStorage.Setup(target => target.GetUser(It.IsAny<long>())).ReturnsAsync(new User());
        
        mailClient.Setup(target => target.GetAuthenticationCode()).ReturnsAsync(
            new AuthenticationData()
            {
                user_code = "code",
                verification_url = "url",
                expires_in = 300,
            }
        );

        mailClient.Setup(target => target.CheckToken(It.IsAny<AuthenticationData>()))
            .ReturnsAsync(new TokenData() { access_token = "token"});
        
        client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));

        await this.command.Execute(context);
        
        this.client.Verify(target => target.SendMessage(2517, "Ура, вы успешно зарегистрировали почту"));
    }
    
        
    [Test]
    public async Task ShouldUpdateTokens()
    {
        mailClient.Setup(target => target.CheckToken(It.IsAny<AuthenticationData>()))
            .ReturnsAsync(new TokenData() { access_token = "access_token", refresh_token = "refresh_token", expires_in = 500 });
        
        mailClient.Setup(target => target.GetAuthenticationCode()).ReturnsAsync(
            new AuthenticationData()
            {
                user_code = "code",
                verification_url = "url",
                expires_in = 300,
            }
        );
        await SkipStep();
        
        var oldUser = new User
        {
            ChatId = 2517,
            Name = "Александр Пушкин"
        };
        userStorage.Setup(obj => obj.GetUser(It.IsAny<long>())).ReturnsAsync(oldUser);
        userStorage.Setup(obj => obj.UpdateUser(It.IsAny<User>()));
        
        await this.command.Execute(context);
        
        userStorage.Verify(target => target.UpdateUser(It.Is<User>(
            user => user.ChatId == 2517 && user.Name == "Александр Пушкин" && user.AccessToken == "access_token" && user.RefreshToken == "refresh_token"
        )));
    }

    private Task SkipStep()
    {
        return this.command.Execute(context);
    }
}