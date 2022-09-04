using Microsoft.Extensions.Logging;
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

    private readonly IMailClient _mailClient;

    private readonly ITelegramClient _telegramClient;

    private CancellationTokenSource _cancellationTokenSource = new();

    public TelegramBot(string telegramToken, MailConfig mailConfig, Database database)
    {
        _database = database;

        _sessionStorage = new SessionStorage();

        _yandexAuthenticator = new YandexAuthenticator(mailConfig);

        _chain = new CommandsListeningChain();

        _mailClient = new MailClient();

        _telegramClient = new TelegramClient(telegramToken, _cancellationTokenSource.Token);

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