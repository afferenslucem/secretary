using Moq;
using Secretary.Telegram.Commands.ExceptionHandlers;
using Secretary.Telegram.Exceptions;

namespace Secretary.Telegram.Tests.Commands.ExceptionHandlers;

public class NonCompleteUserExceptionHandlerTests
{
    private Mock<ITelegramClient> _client = null!;
    private NonCompleteUserExceptionHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _client = new Mock<ITelegramClient>();
        _handler = new NonCompleteUserExceptionHandler();
    }
    
    [Test]
    public async Task ShouldHandleNonRegisteredUser()
    {
        await _handler.Handle(new NonCompleteUserException("User does not exist"), 2517, _client.Object);
        
        _client.Verify(target => target.SendMessage(2517, "Вы – незарегистрированный пользователь.\n\n" +
                                                          "Выполните команды:\n" +
                                                          "/registeruser\n" +
                                                          "/registermail"));
    }
    
    [Test]
    public async Task ShouldHandleUserWithoutMail()
    {
        await _handler.Handle(new NonCompleteUserException("User has not got registered email"), 2517, _client.Object);
        
        _client.Verify(target => target.SendMessage(2517, "У вас не зарегистрирована почта.\n" +
                                                          "Выполните команду: /registermail"));
    }
    
    [Test]
    public async Task ShouldHandleUserWithoutName()
    {
        await _handler.Handle(new NonCompleteUserException("User has not got personal info"), 2517, _client.Object);
        
        _client.Verify(target => target.SendMessage(2517, "У вас не заданы данные о пользователе.\n" +
                                                          "Выполните команду /registeruser"));
    }
}