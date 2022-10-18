using Secretary.Logging;
using Secretary.Storage.Models;
using Secretary.Telegram.Commands.Abstractions;
using Serilog;

namespace Secretary.Telegram.Commands;

public class StartCommand: Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<StartCommand>();
    
    public const string Key = "/start";
    
    public override async Task Execute()
    {
        await RegisterUserOnFirstRunning();
        
        _logger.Information($"{ChatId}: Started work");

        await TelegramClient.SendMessage(
            "Добро пожаловать!\n" +
            "\n" +
            "Перед началом работы вам необходимо:\n" +
            "1. /registeruser – зарегистрироваться\n" +
            "2. /registermail – зарегистрировать рабочую почту\n" +
            "3. <a href=\"https://mail.yandex.ru/#setup/client\">Разрешить доступ по протоколу IMAP</a>"
        );
    }

    private async Task RegisterUserOnFirstRunning()
    {
        _logger.Debug("Check user existing");
        var existingUser = await UserStorage.GetUser();

        if (existingUser == null)
        {
            var user = new User
            {
                ChatId = ChatId,
                TelegramUsername = TelegramUserName,
            };

            await UserStorage.SetUser(user);
            _logger.Debug($"Register user {ChatId}");
        }
        else
        {
            existingUser.TelegramUsername = TelegramUserName;
            await UserStorage.SetUser(existingUser);
            
            _logger.Debug($"User exists");
        }
    }
}