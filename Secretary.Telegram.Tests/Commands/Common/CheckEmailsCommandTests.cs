﻿using Moq;
using Secretary.Cache;
using Secretary.Storage;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Common;

namespace Secretary.Telegram.Tests.Commands.Common;

public class CheckEmailsCommandTests
{
    private Mock<ITelegramClient> _client = null!;
    private Mock<IEmailStorage> _emailStorage = null!;
    private Mock<IDocumentStorage> _documentStorage = null!;
    private Mock<ICacheService> _cacheService = null!;
    
    private CheckEmailsCommand<TimeOffCache> _command = null!;
    private CommandContext _context = null!;

    [SetUp]
    public void Setup()
    {
        this._emailStorage = new Mock<IEmailStorage>();
        this._documentStorage = new Mock<IDocumentStorage>();
        this._cacheService = new Mock<ICacheService>();
        this._client = new Mock<ITelegramClient>();

        this._command = new CheckEmailsCommand<TimeOffCache>();

        this._context = new CommandContext()
        { 
            ChatId = 2517, 
            TelegramClient = _client.Object, 
            EmailStorage = _emailStorage.Object,
            CacheService = _cacheService.Object,
            DocumentStorage = _documentStorage.Object,
        };
        
        this._command.Context = _context;
    }
    
    [Test]
    public async Task ShouldReturnEmailTable()
    {
        _cacheService
            .Setup(target => target.GetEntity<TimeOffCache>(It.IsAny<long>()))
            .ReturnsAsync(new TimeOffCache()
            {
                Emails = new []
                {
                    new Email() { Address = "a.pushkin@infinnity.ru", DisplayName = "Александр Пушкин"},
                    new Email() { Address = "s.esenin@infinnity.ru", DisplayName = "Сергей Есенин"},
                    new Email() { Address = "v.mayakovskii@infinnity.ru"},
                }
            });

        await _command.Execute();
        
        _client.Verify(target => target.SendMessageWithKeyBoard(2517, 
            "Заявление будет отправлено на следующие адреса:\n" +
            "<code>\n" +
            "a.pushkin@infinnity.ru (Александр Пушкин)\n" +
            "s.esenin@infinnity.ru (Сергей Есенин)\n" +
            "v.mayakovskii@infinnity.ru" +
            "</code>\n" +
            "\n" +
            "Все верно?",
            new [] { "Верно", "Нет, нужно поправить" }));
    }
    
    [Test]
    public async Task ShouldSaveNewEmailsOnCorrect()
    {
        _documentStorage.Setup(
                target => target.GetOrCreateDocument(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Document(0, "name"));
        
        var expectedEmails = new[]
        {
            new Email() { Address = "a.pushkin@infinnity.ru", DisplayName = "Александр Пушкин" },
            new Email() { Address = "s.esenin@infinnity.ru", DisplayName = "Сергей Есенин" },
            new Email() { Address = "v.mayakovskii@infinnity.ru" },
        };
        
        _cacheService.Setup(target => target.GetEntity<TimeOffCache>(It.IsAny<long>())).ReturnsAsync(new TimeOffCache()
        {
            Emails = expectedEmails
        });
        
        _context.Message = "верно";

        var result = await _command.OnMessage();
        
        _emailStorage.Verify(target => target.SaveForDocument(0, expectedEmails));
        
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