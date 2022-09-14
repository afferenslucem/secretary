using Secretary.Logging;
using Secretary.Storage.Models;
using Serilog;

namespace Secretary.Telegram.Commands;

public class MeCommand: Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<MeCommand>();
    
    public const string Key = "/me";
    
    public override async Task Execute()
    {
        _logger.Information($"{ChatId}: Asked user info");

        var user = await UserStorage.GetUser();

        if (user == null)
        {
            await ReturnUnregisteredUser();
            return;
        }

        if (user.AccessToken == null)
        {
            await ReturnUnregisteredMail(user);
            return;
        }

        if (user.JobTitleGenitive == null)
        {
            await ReturnUnregisteredName(user);
            return;
        }

        await ReturnUserInfo(user);
    }

    private Task ReturnUnregisteredUser()
    {
        return TelegramClient.SendMessage("Вы незарегистрированный пользователь\n\n" +
                                          "Для корректной работы вам необходимо выполнить следующие команды:\n" +
                                          "/registeruser\n" +
                                          "/registermail");
    }

    private Task ReturnUnregisteredMail(User user)
    {
        var userInfo = GetUserInfo(user);
        
        return TelegramClient.SendMessage($"{userInfo}\n\n" +
                                           "У вас нет токена для почты. Выполните команду /registermail");
    }

    private Task ReturnUnregisteredName(User user)
    {
        var userInfo = GetUserInfo(user);
        
        return TelegramClient.SendMessage($"{userInfo}\n\n" +
                                           "У вас не заданы данные о пользователе. Выполните команду /registeruser");
    }

    private Task ReturnUserInfo(User user)
    {
        var userInfo = GetUserInfo(user);
        
        return TelegramClient.SendMessage(userInfo);
    }

    private string GetUserInfo(User user)
    {
        var result = $"<strong>Имя:</strong> {user.Name ?? "не задано"}\n" +
                   $"<strong>Имя в Р.П.:</strong> {user.NameGenitive ?? "не задано"}\n" +
                   $"<strong>Должность:</strong> {user.JobTitle ?? "не задана"}\n" +
                   $"<strong>Должность в Р.П.:</strong> {user.JobTitleGenitive ?? "не задана"}\n" +
                   $"<strong>Почта:</strong> {user.Email ?? "не задана"}";

        return result;
    }
}