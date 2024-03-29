﻿using Moq;
using Secretary.Cache;
using Secretary.Storage.Interfaces;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Caches.Documents;
using Secretary.Telegram.Commands.Documents.Common;

namespace Secretary.Telegram.Tests.Commands.Documents.Common;

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
        _emailStorage = new Mock<IEmailStorage>();
        _documentStorage = new Mock<IDocumentStorage>();
        _cacheService = new Mock<ICacheService>();
        _client = new Mock<ITelegramClient>();

        _command = new CheckEmailsCommand<TimeOffCache>();

        _context = new CommandContext()
        { 
            UserMessage = new UserMessage { ChatId = 2517},
            TelegramClient = _client.Object, 
            EmailStorage = _emailStorage.Object,
            CacheService = _cacheService.Object,
            DocumentStorage = _documentStorage.Object,
        };
        
        _command.Context = _context;
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
        
        _client.Verify(target => target.SendMessage(2517, 
            "Заявление будет отправлено на следующие адреса:\n" +
            "<code>\n" +
            "a.pushkin@infinnity.ru (Александр Пушкин)\n" +
            "s.esenin@infinnity.ru (Сергей Есенин)\n" +
            "v.mayakovskii@infinnity.ru" +
            "</code>\n" +
            "\n" +
            "Все верно?",
            TestUtils.ItIsReplayKeyBoard(new [] { "Верно", "Нет, нужно поправить" })));
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
        
        _context.UserMessage.Text ="верно";

        var result = await _command.OnMessage();
        
        _emailStorage.Verify(target => target.SaveForDocument(0, expectedEmails));
        
        Assert.That(result, Is.EqualTo(1));
    }
    
    [Test]
    public async Task ShouldReturnRunPrevOnIncorrect()
    {
        _context.UserMessage.Text ="не верно";
        var result = await _command.OnMessage();
        
        Assert.That(result, Is.EqualTo(-1));
    }
}