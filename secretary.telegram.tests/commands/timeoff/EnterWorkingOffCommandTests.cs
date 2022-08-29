﻿using Moq;
using secretary.configuration;
using secretary.documents;
using secretary.storage;
using secretary.storage.models;
using secretary.telegram.commands;
using secretary.telegram.commands.timeoff;
using secretary.telegram.sessions;

namespace secretary.telegram.tests.commands.subcommands.timeoff;

public class EnterWorkingOffCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    
    private TimeOffCommand _parent = null!;
    private EnterWorkingOffCommand _command = null!;
    private CommandContext _context = null!;
        
    [SetUp]
    public void Setup()
    {
        this._client = new Mock<ITelegramClient>();

        this._command = new EnterWorkingOffCommand();

        this._parent = new TimeOffCommand();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = this._client.Object, 
        };
    }
    
    [Test]
    public async Task ShouldSendEnterWorkingOffCommand()
    {
        _client.Setup(obj => obj.SendMessage(It.IsAny<long>(), It.IsAny<string>()));
        
        await this._command.Execute(_context);
        
        this._client.Verify(target => target.SendMessageWithKeyBoard(2517, 
            "Введите данные об отработке в свободном формате.\r\n" +
            "Например: <i>Отгул обязуюсь отработать</i>\r\n" +
            "Или: Отгул <i>без отработки</i>\r\n\r\n" +
            "Если вы нажмете \"Пропустить\", то эти данные просто не будут указаны в документе.",
            new [] { "Пропустить" }));
    }
    
    [Test]
    public async Task ShouldSetWorkingOffToCommand()
    {
        _context.Message = "Отгул обязуюсь отработать";
        
        await this._command.OnMessage(_context, _parent);
        
        Assert.That(_context.Message, Is.EqualTo(this._parent.Data.WorkingOff));
    }
    
    [Test]
    public async Task ShouldSkipWorkingOffCommand()
    {
        _context.Message = "Пропустить";
        
        await this._command.OnMessage(_context, _parent);
        
        Assert.IsNull(this._parent.Data.WorkingOff);
    }
}