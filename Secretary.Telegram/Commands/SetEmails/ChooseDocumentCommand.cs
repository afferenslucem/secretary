using Secretary.Logging;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Validation;
using Secretary.Telegram.Documents;
using Secretary.Telegram.Exceptions;
using Serilog;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Commands.SetEmails;

public class ChooseDocumentCommand: Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<ChooseDocumentCommand>();
    public override async Task Execute()
    {
        await ValidateUser();

        var allDocs = DocumentContextProvider.AllDocuments
            .Select(item => InlineKeyboardButton.WithCallbackData(item.MailTheme))
            .Select(item => new [] { item })
            .ToArray();

        await TelegramClient.SendMessage(
            "Выберете документ для установки получателей", 
            allDocs
        );
    }

    public override async Task<int> OnMessage()
    {
        try
        {
            var context = DocumentContextProvider
                .AllDocuments
                .First(item => item.MailTheme == Message);

            await TelegramClient.SendMessage($"Выбран документ \"{context.MailTheme}\"");

            var cache = new SetEmailsCache
            {
                DocumentKey = context.Key
            };

            await CacheService.SaveEntity(cache);

            return ExecuteDirection.RunNext;
        }
        catch (InvalidOperationException  e)
        {
            _logger.Warning(e, "Incorrect document name");
            throw new IncorrectMessageException();
        }
    }

    private async Task ValidateUser()
    {
        var user = await UserStorage.GetUser();
        
        new UserValidationVisitor().Validate(user);
    }
}