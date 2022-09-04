using Microsoft.Extensions.Logging;
using secretary.cache;
using secretary.configuration;
using secretary.logging;
using secretary.mail.Authentication;
using secretary.storage;
using secretary.telegram.chains;
using secretary.telegram.commands;
using secretary.telegram.commands.executors;
using secretary.telegram.sessions;
using secretary.yandex.mail;

namespace secretary.telegram;

public class TelegramBot
{
    public const string Version = "v0.1.0";
    
    private readonly ILogger<TelegramBot> _logger = LogPoint.GetLogger<TelegramBot>();
    
    private readonly CommandsListeningChain _chain;

    private readonly ISessionStorage _sessionStorage;

    private readonly Database _database;

    private readonly IYandexAuthenticator _yandexAuthenticator;
    
    private readonly ICacheService _cacheService;

    private readonly IMailClient _mailClient;

    private readonly ITelegramClient _telegramClient;

    private CancellationTokenSource _cancellationTokenSource = new();

    public TelegramBot(Config config, Database database)
    {
        _database = database;

        _cacheService = new RedisCacheService(config.RedisHost);

        _sessionStorage = new SessionStorage(_cacheService);

        _yandexAuthenticator = new YandexAuthenticator(config.MailConfig);

        _chain = new CommandsListeningChain();

        _mailClient = new MailClient();

        _telegramClient = new TelegramClient(config.TelegramApiKey, _cancellationTokenSource.Token);

        _telegramClient.OnMessage += this.WorkWithMessage;
    }

    private async Task WorkWithMessage(BotMessage message)
    {
        var command = _chain.Get(message.Text)!;
        
        try
        {
            LogCommand(command, $"{message.ChatId}: execute command {command.GetType().Name}");

            var context = new CommandContext(
                message.ChatId,
                _telegramClient,
                _sessionStorage,
                _database.UserStorage,
                _database.DocumentStorage,
                _database.EmailStorage,
                _yandexAuthenticator,
                _mailClient,
                _cacheService,
                message.Text
            );

            await new CommandExecutor(command, context).Execute();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{message.ChatId}: Сommand execution fault {command.GetType().Name}");
        }
    }
    
    public Task Listen()
    {
        _logger.LogInformation("Start work");
        return this._telegramClient.RunDriver();
    }

    public void Cancel()
    {
        this._cancellationTokenSource.Cancel();
    }

    public void LogCommand(Command command, string message)
    {
        if (command.GetType() == typeof(NullCommand)) return;
        
        _logger.LogInformation(message);
    }
}