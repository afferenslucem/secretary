using secretary.configuration;
using secretary.mail;
using secretary.mail.Authentication;
using secretary.telegram.chains;
using secretary.telegram.commands;
using secretary.telegram.sessions;
using secretary.storage;
using secretary.yandex.mail;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace secretary.telegram;

public class TelegramBot: ITelegramClient
{
    private CancellationTokenSource _cancellationToken = new CancellationTokenSource();
    
    private TelegramBotClient _telegramClient;

    private CommandsListeningChain _chain;

    private ISessionStorage _sessionStorage;

    private Database _database;

    private IYandexAuthenticator _yandexAuthenticator;
    private IMailClient _mailClient;

    public TelegramBot(string token, MailConfig mailConfig, Database database)
    {
        this._database = database;

        this._telegramClient = new TelegramBotClient(token);

        this._sessionStorage = new SessionStorage();

        this._yandexAuthenticator = new YandexAuthenticator(mailConfig);

        this._chain = new CommandsListeningChain();

        this._mailClient = new MailClient();
    }

    public async Task Listen()
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new [] { UpdateType.Message }
        };
        
        while (!this._cancellationToken.IsCancellationRequested)
        {
            await this._telegramClient.ReceiveAsync(
                updateHandler: this.HandleUpdateAsync,
                pollingErrorHandler: this.HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: this._cancellationToken.Token);
        }
    }
    
    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;

        if (message?.Text == null)
        {
            return;
        }

        var command = this._chain.Get(message.Text)!;

        try
        {
            await command.Execute(new CommandContext(
                message.Chat.Id,
                this,
                this._sessionStorage,
                this._database.UserStorage,
                this._database.DocumentStorage,
                this._database.EmailStorage,
                this._yandexAuthenticator,
                this._mailClient,
                message.Text
            ));
            
            Console.WriteLine($"Executed command { command.GetType().Name }");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    public Task SendMessage(long chatId, string message)
    {
        return this._telegramClient.SendTextMessageAsync(chatId, message, cancellationToken: this._cancellationToken.Token, parseMode: ParseMode.Html);
    }

    public Task SendMessageWithKeyBoard(long chatId, string message, string[] choises)
    {
        var buttons = choises.Select(text => new KeyboardButton(text)).ToArray();
        
        var keyboard = new ReplyKeyboardMarkup(buttons);
        keyboard.OneTimeKeyboard = true;
        
        return this._telegramClient.SendTextMessageAsync(chatId, message, parseMode: ParseMode.Html, replyMarkup: keyboard, cancellationToken: this._cancellationToken.Token);
    }

    public async Task SendDocument(long chatId, string path, string fileName)
    {
        await using var fileStream = File.OpenRead(path);
        
        await this._telegramClient.SendDocumentAsync(
            chatId: chatId,
            document: new InputOnlineFile(content: fileStream, fileName: fileName), cancellationToken: this._cancellationToken.Token);
    }
}