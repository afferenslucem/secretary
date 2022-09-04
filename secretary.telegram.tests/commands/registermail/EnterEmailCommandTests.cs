﻿using Moq;
using secretary.cache;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.registermail;
using secretary.telegram.exceptions;

namespace secretary.telegram.tests.commands.registermail;

public class EnterEmailCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IUserStorage> _userStorage = null!;
    private Mock<ICacheService> _cacheService = null!;
    private CommandContext _context= null!;
    private EnterEmailCommand _command= null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();
        this._userStorage = new Mock<IUserStorage>();
        this._cacheService = new Mock<ICacheService>();

        this._context = new CommandContext()
        {
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
            UserStorage = _userStorage.Object,
            CacheService = _cacheService.Object,
        };

        this._command = new EnterEmailCommand();
        this._command.Context = _context;
    }

    [Test]
    public void ShouldCreate()
    {
        Assert.Pass();
    }
    
    [Test]
    public async Task ShouldSendEnterEmail()
    {
        await this._command.Execute();
        
        this._client.Verify(target => target.SendMessage(2517, "Введите вашу почту, с которой вы отправляете заявления.\r\n" +
                                                              @"Например: <i>a.pushkin@infinnity.ru</i>"));
    }
    
    [Test]
    public async Task ShouldSetCacheDataEmail()
    {
        _context.Message = "a.pushkin@infinnity.ru";
        
        await this._command.OnMessage();
        
        this._cacheService.Verify(target => target.SaveEntity(2517,  new RegisterMailData("a.pushkin@infinnity.ru"), It.IsAny<short>()));
    }
        
    [Test]
    public void ShouldThrowErrorForIncorrectEmail()
    {
        _context.Message = "a.pushkin@infinnity";

        _command.Context = _context;
        
        Assert.ThrowsAsync<IncorrectFormatException>(async () => await this._command.ValidateMessage());
        _client.Verify(target => target.SendMessage(2517, "Некорректный формат почты. Введите почту еще раз"));
    }
}