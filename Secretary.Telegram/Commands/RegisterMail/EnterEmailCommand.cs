using System.Text.RegularExpressions;
using Secretary.Logging;
using Secretary.Telegram.Commands.Caches;
using Secretary.Telegram.Exceptions;
using Serilog;

namespace Secretary.Telegram.Commands.RegisterMail;

public class EnterEmailCommand : Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<EnterEmailCommand>();
    public override Task Execute()
    {
        _logger.Information($"{ChatId}: started register mail");
        
        return TelegramClient.SendMessage("Введите вашу почту, с которой вы отправляете заявления.\n" +
                                          @"Например: <i>a.pushkin@infinnity.ru</i>");
    }

    public override async Task<int> OnMessage()
    {
        var cache = new RegisterMailCache(Message);

        await CacheService.SaveEntity(cache);
        
        return ExecuteDirection.RunNext;
    }

    public override async Task ValidateMessage()
    {
        var emailRegex = new Regex(@"^[\w_\-\.]+@([\w\-_]+\.)+[\w-]{2,4}");
        if (!emailRegex.IsMatch(Message))
        {
            await TelegramClient.SendMessage("Некорректный формат почты. Введите почту еще раз");
            throw new IncorrectMessageException();
        }

        var domainAllowed = YandexAuthenticator.IsUserDomainAllowed(Message);

        if (!domainAllowed)
        {
            await TelegramClient.SendMessage("Некорректный домен почты.\n" +
                                             "Бот доступен только для сотрудников Infinnity Solutions.\n" +
                                             "Введите вашу рабочую почту");
            throw new IncorrectMessageException();
        }
    }
}