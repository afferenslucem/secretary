using Secretary.Logging;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace Secretary.Telegram;

public class TelegramClient: ITelegramClient
{
    private readonly ILogger _logger = LogPoint.GetLogger<TelegramClient>();
    public event MessageReceive? OnMessage;
    
    private readonly TelegramBotClient _botClient;
    
    private readonly CancellationToken _cancellationToken;

    public DateTime LastCheckTime { get; set; }

    public TelegramClient(string token, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _botClient = new TelegramBotClient(token);
    }

    public async Task RunDriver()
    {
        _logger.Information("Started listen");
        
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new [] { UpdateType.Message, UpdateType.CallbackQuery }
        };

        _botClient.OnApiResponseReceived += SaveLifeTime;
        
        while (!_cancellationToken.IsCancellationRequested)
        {
            await _botClient.ReceiveAsync(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: _cancellationToken);
        }
    }
    
    Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        LastCheckTime = DateTime.UtcNow;

        var botMessage = GetMessageFromUpdate(update);
        
        _logger.Debug($"({botMessage.ChatId}): {botMessage.Text}");
        
        var task = OnMessage?.Invoke(botMessage);

        return task ?? Task.CompletedTask;
    }

    private BotMessage? GetMessageFromUpdate(Update update)
    {
        var message = update.Message;

        if (message?.Text != null)
        {
            return new BotMessage(message.Chat.Id, message.Text);
        }

        var callback = update.CallbackQuery;

        if (callback?.Data != null)
        {
            return new BotMessage(callback.From.Id, callback.Data);
        }

        return null;
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
    
    public Task SendMessage(
        long chatId, 
        string message
    ) {
        return _botClient.SendTextMessageAsync(
            chatId,
            message, 
            cancellationToken: _cancellationToken, 
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove()
        );
    }
    
    public Task SendMessage(
        long chatId, 
        string message, 
        ReplyKeyboardMarkup replyMarkup
    ) {
        replyMarkup.ResizeKeyboard = true;
        replyMarkup.OneTimeKeyboard = true;
        
        return _botClient.SendTextMessageAsync(
            chatId,
            message, 
            cancellationToken: _cancellationToken, 
            parseMode: ParseMode.Html, 
            replyMarkup: replyMarkup
        );
    }
    
    public Task SendMessage(
        long chatId, 
        string message,
        InlineKeyboardMarkup inlineKeyboardMarkup
    ) {
        return _botClient.SendTextMessageAsync(
            chatId,
            message, 
            cancellationToken: _cancellationToken, 
            parseMode: ParseMode.Html, 
            replyMarkup: inlineKeyboardMarkup
        );
    }

    public async Task SendDocument(long chatId, string path, string fileName)
    {
        await using var fileStream = File.OpenRead(path);
        
        await _botClient.SendDocumentAsync(
            chatId: chatId,
            document: new InputOnlineFile(content: fileStream, fileName: fileName), cancellationToken: _cancellationToken);
    }

    public async Task SendSticker(long chatId, string stickerId)
    {
        await _botClient.SendStickerAsync(chatId, new InputOnlineFile(stickerId));
    }

    private ValueTask SaveLifeTime(ITelegramBotClient client, ApiResponseEventArgs e, CancellationToken token)
    {
        this.LastCheckTime = DateTime.UtcNow;
        
        _logger.Debug("Received answer");
        
        return ValueTask.CompletedTask;
    }
}