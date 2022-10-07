using Secretary.Logging;
using Secretary.Telegram.Commands.Caches.Interfaces;
using Secretary.Telegram.Commands.Validation;
using Secretary.Telegram.Documents;
using Secretary.Telegram.Exceptions;
using Secretary.Telegram.Utils;
using Serilog;

namespace Secretary.Telegram.Commands.TimeOff;

public class EnterTimeOffPeriodCommand<T> : Command
    where T : class, IPeriodCache, new()
{
    private readonly ILogger _logger = LogPoint.GetLogger<EnterTimeOffPeriodCommand<T>>();

    public override async Task Execute()
    {
        await ValidateUser();

        var context = DocumentContextProvider.GetContext(TimeOffCommand.Key);
        await TelegramClient.SendMessage($"Вы выбрали документ \"{context.MailTheme}\"");
        
        _logger.Information($"{ChatId}: Started time off");
        await TelegramClient.SendMessage("Введите период отгула в одном из форматов:\n\n" +
                                         "Если отгул на один день:\n<strong>DD.MM.YYYY[ с HH:mm до HH:mm]</strong>\n" +
                                         "Например: <i>26.04.2020 c 9:00 до 13:00</i>\n\n" +
                                         "Или, если отгул на несколько дней:\n<strong>с [HH:mm ]DD.MM.YYYY до [HH:mm ]DD.MM.YYYY</strong>\n" +
                                         "Например: <i>с 9:00 26.04.2020 до 13:00 28.04.2022</i>\n\n" +
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