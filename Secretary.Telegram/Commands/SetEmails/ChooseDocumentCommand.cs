using Secretary.Logging;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Validation;
using Secretary.Telegram.Documents;
using Secretary.Telegram.Exceptions;
using Serilog;

namespace Secretary.Telegram.Commands.SetEmails;

public class ChooseDocumentCommand: Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<ChooseDocumentCommand>();
    public override async Task Execute()
    {
        await ValidateUser();
        
        var allDocs = DocumentContextProvider.AllDocuments.Select(item => item.MailTheme);

        await TelegramClient.SendMessageWithKeyBoard("Выберете документ для установки получателей", allDocs.Chunk(2).ToArray());
    }

    public override async Task<int> OnMessage()
    {
        try
        {
            var key = DocumentContextProvider
                .AllDocuments
                .First(item => item.MailTheme == Message)
                .Key;

            var cache = new SetEmailsCache
            {
                DocumentKey = key
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