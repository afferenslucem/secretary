﻿using Moq;
using secretary.configuration;
using secretary.documents;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.timeoff;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands.subcommands.timeoff;

public class EnterReasonCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    
    private TimeOffCommand _parent = null!;
    private EnterReasonCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._command = new EnterReasonCommand();

        this._parent = new TimeOffCommand();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
        };
    }
    
    [Test]
    public async Task ShouldSendEnterReasonCommand()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        
        await this._command.Execute(_context);
        
        this._client.Verify(target => target.SendMessageWithKeyBoard(2517, 
            "Введите причину отгула, это опционально, если вы нажмете \"Пропустить\", то этой графы не будет в заявлении.\r\n" +
            "А если укажете, то это будет строка вида <code>Причина: {{причина}}</code>",
            new [] {"Пропустить"}));
    }
    
    [Test]
    public async Task ShouldSetReasonToCommand()
    {
        _context.Message = "Поеду заниматься ремонтом";

        await this._command.OnMessage(_context, _parent);
        
        Assert.That(_context.Message, Is.EqualTo(this._parent.Data.Reason));
    }
    
    
    [Test]
    public async Task ShouldSkipReasonToCommand()
    {
        _context.Message = "Пропустить";
        
        await this._command.OnMessage(_context, _parent);
        
        Assert.IsNull(_parent.Data.Reason);
    }
}