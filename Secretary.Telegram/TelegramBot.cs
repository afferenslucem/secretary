
using Secretary.Cache;
using Secretary.Configuration;
using Secretary.Logging;
using Secretary.Storage;
using Secretary.Storage.Models;
using Secretary.Telegram.chains;
using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.ExceptionHandlers;
using Secretary.Telegram.Commands.Executors;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Sessions;
using Secretary.Yandex.Authentication;
using Secretary.Yandex.Mail;
using Serilog;

namespace Secretary.Telegram;

public class TelegramBot
{
    public static readonly string Version = "v2.6.1";
    
    public static readonly DateTime Uptime = DateTime.Now;

    private Config _config;
    
    private ILogger _logger = LogPoint.GetLogger<TelegramBot>();
    
    private CommandsListeningChain _chain;

    private ISessionStorage _sessionStorage;

    private Database _database;

    private IYandexAuthenticator _yandexAuthenticator;
    
    private ICacheService _cacheService;

    private IMailClient _mailClient;

    public virtual ITelegramClient TelegramClient { get; set; }

    private CancellationTokenSource _cancellationTokenSource = new();

    private TokenRefresher _refresher;
    
    public TelegramBot(
        Config config, 
        Database database
    ) {
        _database = database;
        _config = config;
    }

    public void Init()
    {
        _cacheService = new RedisCacheService(_config.RedisHost);

        _sessionStorage = new SessionStorage(_cacheService);

        _yandexAuthenticator = new YandexAuthenticator(_config.MailConfig);

        _chain = new CommandsListeningChain();

        _mailClient = new MailClient();

        TelegramClient = new TelegramClient(_config.TelegramApiKey, _cancellationTokenSource.Token);

        TelegramClient.OnMessage += WorkWithMessage;

        _refresher = new TokenRefresher(
            _yandexAuthenticator, 
            TelegramClient, 
            _database.UserStorage, 
            _cancellationTokenSource.Token
        );

        _refresher.OnUserInvalidToken += HandleUserTokenExpired;
    }

    public async Task HandleUserTokenExpired(User user)
    {
        await _database.UserStorage.RemoveTokens(user.ChatId);
        await TelegramClient.SendMessage(
            user.ChatId, 
            "У вас истек токен для отправки почты!\n\n" +
                   $"Выполните команду /registermail для адреса {user.Email}"
            );
    }

    private async Task WorkWithMessage(BotMessage message)
    {
        var command = _chain.Get(message.Text)!;
        
        try
        {
            LogCommand(command, $"{message.ChatId}: execute command {command.GetType().Name}");

            var context = new CommandContext(
                message.ChatId,
                TelegramClient,
                _sessionStorage,
                _database.UserStorage,
                _database.DocumentStorage,
                _database.EmailStorage,
                _database.EventLogStorage,
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
            await CatchException(e, message.ChatId);
            
            _logger.Error(e, $"{message.ChatId}: Сommand execution fault {command.GetType().Name}");
            await _sessionStorage.DeleteSession(message.ChatId);
        }
    }
    
    public Task Listen()
    {
        _logger.Information($"Version: {Version}");
        _logger.Information($"Uptime: {Uptime}");

        _refresher.RunThread();
        
        return TelegramClient.RunDriver();
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

    private async Task CatchException(Exception e, long chatId)
    {
        if (e is NonCompleteUserException nonCompleteUserException)
        {
            await new NonCompleteUserExceptionHandler().Handle(nonCompleteUserException, chatId, TelegramClient);
        }
    }
}
