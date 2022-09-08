using Microsoft.Extensions.Logging;
using secretary.logging;

namespace secretary.telegram.commands;

public class StartCommand: Command
{
    private readonly ILogger<StartCommand> _logger = LogPoint.GetLogger<StartCommand>();
    
    public const string Key = "/start";
    
    public override async Task Execute()
    {
        _logger.LogInformation($"{ChatId}: Started work");

        await TelegramClient.SendMessage(
            "Добро пожаловать!\n" +
            "\n" +
            "Перед началом работы вам необходимо:\n" +
            "/registeruser – зарегистрироваться\n" +
            "/registermail – зарегистрировать рабочую почту");
    }
}