using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.telegram.commands.caches;
using secretary.telegram.exceptions;

namespace secretary.telegram.commands.registermail;

public class EnterEmailCommand : Command
{
    private readonly ILogger<EnterEmailCommand> _logger = LogPoint.GetLogger<EnterEmailCommand>();
    public override Task Execute()
    {
        _logger.LogInformation($"{ChatId}: started register mail");
        
        return TelegramClient.SendMessage("Введите вашу почту, с которой вы отправляете заявления.\r\n" +
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
            throw new IncorrectFormatException();
        }

        var domainAllowed = YandexAuthenticator.IsUserDomainAllowed(Message);

        if (!domainAllowed)
        {
            await TelegramClient.SendMessage("Некорректный домен почты.\r\n" +
                                             "Бот доступен только для сотрудников Infinnity Solutions.\r\n" +
                                             "Введите вашу рабочую почту");
            throw new IncorrectFormatException();
        }
    }
}