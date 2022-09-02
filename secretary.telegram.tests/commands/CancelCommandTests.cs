﻿using Moq;
using secretary.telegram.commands;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands;

public class CancelCommandTests
{
    private Mock<ISessionStorage> _sessionStorage = null!;
    private Mock<ITelegramClient> _client = null!;
    
    private CancelCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._sessionStorage = new Mock<ISessionStorage>();
        this._client = new Mock<ITelegramClient>();

        this._command = new CancelCommand();
        
        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            SessionStorage = this._sessionStorage.Object, 
            TelegramClient = this._client.Object,
        };

        this._command.Context = _context;
    }
    
    [Test]
    public async Task ShouldBreakCommand()
    {
        await this._command.Execute();
        
        this._sessionStorage.Verify(target => target.DeleteSession(2517));
        this._client.Verify(target => target.SendMessage(2517, "Дальнейшее выполнение команды прервано"));
    }
    
    [Test]
    public async Task ShouldCancelCommand()
    {
        var commandMock = new Mock<Command>();

        var session = new Session()
        {
            LastCommand = commandMock.Object,
        };

        _sessionStorage.Setup(target => target.GetSession(It.IsAny<long>())).ReturnsAsync(session);
        
        await this._command.Execute();
        
        commandMock.Verify(target => target.Cancel(), Times.Once);
    }
    
    [Test]
    public async Task ShouldCancelEmptySession()
    {
        var session = new Session();

        _sessionStorage.Setup(target => target.GetSession(It.IsAny<long>())).ReturnsAsync(session);
        
        await this._command.Execute();
    }
}