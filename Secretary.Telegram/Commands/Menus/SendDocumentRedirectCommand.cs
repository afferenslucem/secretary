using Secretary.Logging;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Documents;
using Serilog;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Commands.Menus;

public class SendDocumentRedirectCommand: Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<StartCommand>();
    
    public const string Key = "/senddocument";
    
    public override async Task Execute()
    {
        _logger.Information($"{ChatId}: Choose document");

        var keyboard = DocumentContextProvider.AllDocuments
            .Select(item => InlineKeyboardButton.WithCallbackData(item.MailTheme, item.Key))
            .Select(item => new [] { item })
            .ToArray();
        
        await TelegramClient.SendMessage("Выберите документ для отправки",keyboard);
    }
}