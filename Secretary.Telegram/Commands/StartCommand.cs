using Secretary.Logging;
using Serilog;

namespace Secretary.Telegram.Commands;

public class StartCommand: Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<StartCommand>();
    
    public const string Key = "/start";
    
    public override async Task Execute()
    {
        _logger.Information($"{ChatId}: Started work");

        await TelegramClient.SendMessage(
            "Добро пожаловать!\n" +
            "\n" +
            "Перед началом работы вам необходимо:\n" +
            "1. /registeruser – зарегистрироваться\n" +
            "2./registermail – зарегистрировать рабочую почту\n" +
            "3. <a href=\"https://mail.yandex.ru/#setup/client\">Разрешить доступ по протоколу IMAP</a>"
        );
    }
}