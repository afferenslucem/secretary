
using Secretary.Cache;
using Secretary.Configuration;
using Secretary.Logging;
using Secretary.Storage;
using Secretary.Telegram.chains;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Executors;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Sessions;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Mail;
using Serilog;

namespace Secretary.Telegram;

public class TelegramBot
{
    public const string Version = "v1.2.0";
    
    public static readonly DateTime Uptime = DateTime.Now;
    
    private readonly ILogger _logger = LogPoint.GetLogger<TelegramBot>();
    
    private readonly CommandsListeningChain _chain;

    private readonly ISessionStorage _sessionStorage;

    private readonly Database _database;

    private readonly IYandexAuthenticator _yandexAuthenticator;
    
    private readonly ICacheService _cacheService;

    private readonly IMailClient _mailClient;

    private readonly ITelegramClient _telegramClient;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

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
            await new CommandExecutor(command, context).OnComplete();
        }
        catch (ForceCompleteCommandException e)
        {
            _logger.Error(e, $"{message.ChatId}: Сommand execution fault {command.GetType().Name}");
        }
        catch (Exception e)
        {
            _logger.Error(e, $"{message.ChatId}: Сommand execution fault {command.GetType().Name}");
            await _sessionStorage.DeleteSession(message.ChatId);
        }
    }
    
    public Task Listen()
    {
        _logger.Information("Start work");
        return this._telegramClient.RunDriver();
    }

    public void Cancel()
    {
        this._cancellationTokenSource.Cancel();
    }

    public void LogCommand(Command command, string message)
    {
        if (command.GetType() == typeof(NullCommand)) return;
        
        _logger.Information(message);
    }
}
