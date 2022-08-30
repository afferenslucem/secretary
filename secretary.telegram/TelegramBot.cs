using secretary.configuration;
using secretary.mail.Authentication;
using secretary.storage;
using secretary.telegram.chains;
using secretary.telegram.commands;
using secretary.telegram.sessions;
using secretary.yandex.mail;

namespace secretary.telegram;

public class TelegramBot
{
    private readonly CommandsListeningChain _chain;

    private readonly ISessionStorage _sessionStorage;

    private readonly Database _database;

    private readonly IYandexAuthenticator _yandexAuthenticator;
    private readonly IMailClient _mailClient;

    private readonly ITelegramClient _telegramClient;

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public TelegramBot(string telegramToken, MailConfig mailConfig, Database database)
    {
        this._database = database;

        this._sessionStorage = new SessionStorage();

        this._yandexAuthenticator = new YandexAuthenticator(mailConfig);

        this._chain = new CommandsListeningChain();

        this._mailClient = new MailClient();

        this._telegramClient = new TelegramClient(telegramToken, _cancellationTokenSource.Token);

        this._telegramClient.OnMessage += this.WorkWithMessage;
    }

    private void WorkWithMessage(BotMessage message)
    {
        var command = this._chain.Get(message.Text)!;
        
        var execution = command.Execute(new CommandContext(
            message.ChatId,
            this._telegramClient,
            this._sessionStorage,
            this._database.UserStorage,
            this._database.DocumentStorage,
            this._database.EmailStorage,
            this._yandexAuthenticator,
            this._mailClient,
            message.Text
        ));
        
        execution.ContinueWith(_ =>
        {
            Console.WriteLine($"Executed command {command.GetType().Name}");
        });

        execution.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Console.WriteLine(task.Exception);
            }
        });
    }
    
    public Task Listen()
    {
        return this._telegramClient.RunDriver();
    }

    public void Cancel()
    {
        this._cancellationTokenSource.Cancel();
    }
}