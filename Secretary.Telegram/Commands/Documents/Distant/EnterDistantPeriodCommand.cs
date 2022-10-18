using Secretary.Logging;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Caches.Documents.Interfaces;
using Secretary.Telegram.Commands.Validation;
using Secretary.Telegram.Documents;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Utils;
using Serilog;

namespace Secretary.Telegram.Commands.Documents.Distant;

public class EnterDistantPeriodCommand<T> : Command
    where T : class, IPeriodCache, new()
{
    private readonly ILogger _logger = LogPoint.GetLogger<EnterDistantPeriodCommand<T>>();

    public override async Task Execute()
    {
        await ValidateUser();
        
        _logger.Information($"{ChatId}: Started time off");
        
        var context = DocumentContextProvider.GetContext(DistantCommand.Key);
        await TelegramClient.SendMessage($"Вы выбрали документ \"{context.MailTheme}\"");

        await TelegramClient.SendMessage("Введите период удаленной работы в одном из форматов:\n\n" +
                                         "Если на один день:\n<strong>DD.MM.YYYY[ с HH:mm до HH:mm]</strong>\n" +
                                         "Например: <i>26.04.2020 c 9:00 до 13:00</i>\n\n" +
                                         "Или, если на несколько дней:\n<strong>с DD.MM.YYYY до DD.MM.YYYY</strong>\n" +
                                         "Например: <i>с 26.04.2020 до 28.04.2022</i>\n\n" +
                                         "Или, на неопределенный срок:\n<strong>с DD.MM.YYYY до DD.MM.YYYY</strong>\n" +
                                         "Например: <i>с 26.04.2020 на неопределенный срок</i>\n\n" +
                                         "В таком виде это будет вставлено в документ");
    }
    
    public override async Task<int> OnMessage()
    {
        try
        {
            var cache = new T();

            var period = new DatePeriodParser().Parse(Message);

            cache.Period = period;

            await CacheService.SaveEntity(cache);

            return ExecuteDirection.RunNext;
        }
        catch (DatePeriodParseException e)
        {
            await HandleIncorrectDate(e);

            throw new IncorrectMessageException();
        }
        catch (FormatException e)
        {
            await HandleIncorrectDate(e);

            throw new IncorrectMessageException();
        }
    }

    private async Task HandleIncorrectDate(Exception e)
    {
        _logger.Warning(e, "Could not parse period");

        await TelegramClient.SendMessage("Неверный формат периода отгула!\n" +
                                         "Попробуйте еще раз");
    }
    
    private async Task ValidateUser()
    {
        var user = await UserStorage.GetUser();
        
        new UserValidationVisitor().Validate(user);
    }
}