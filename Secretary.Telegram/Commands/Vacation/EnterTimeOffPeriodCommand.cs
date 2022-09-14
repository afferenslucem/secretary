using Secretary.Logging;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Commands.Caches.Interfaces;
using Secretary.Telegram.Commands.Validation;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Utils;
using Serilog;

namespace Secretary.Telegram.Commands.Vacation;

public class EnterVacationPeriodCommand<T> : Command
    where T : class, IPeriodCache, new()
{
    private readonly ILogger _logger = LogPoint.GetLogger<EnterVacationPeriodCommand<T>>();

    public override async Task Execute()
    {
        await ValidateUser();
        
        _logger.Information($"{ChatId}: Started vacation");

        await TelegramClient.SendMessage("Введите период отпуска в формате:\n" +
                                         "<strong> с DD.MM.YYYY по DD.MM.YYYY</strong>\n" +
                                         "Например: <i>с 07.02.2022 по 13.02.2022</i>\n\n" +
                                         "В таком виде это будет вставлено в документ");
    }
    
    public override async Task<int> OnMessage()
    {
        try
        {
            var cache = new T();

            var period = new DatePeriodParser().Parse(Message);

            cache.Period = period;

            if (cache.Period.IsOneDay)
            {
                throw new DatePeriodParseException(Message);
            }

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

        await TelegramClient.SendMessage("Неверный формат периода отпуска!\n" +
                                         "Попробуйте еще раз");
    }
    
    private async Task ValidateUser()
    {
        var user = await UserStorage.GetUser();
        
        new UserValidationVisitor().Validate(user);
    }
}