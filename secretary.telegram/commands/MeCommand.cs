using Microsoft.Extensions.Logging;
using secretary.logging;
using secretary.storage.models;

namespace secretary.telegram.commands;

public class MeCommand: Command
{
    private readonly ILogger<MeCommand> _logger = LogPoint.GetLogger<MeCommand>();
    
    public const string Key = "/me";
    
    public override async Task Execute()
    {
        _logger.LogInformation($"{ChatId}: Asked user info");

        var user = await Context.UserStorage.GetUser(ChatId);

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
        return TelegramClient.SendMessage("Вы незарегистрированный пользователь\r\n\r\n" +
                                          "Для корректной работы вам необходимо выполнить следующие команды:\r\n" +
                                          "/registeruser\r\n" +
                                          "/registermail");
    }

    private Task ReturnUnregisteredMail(User user)
    {
        var userInfo = GetUserInfo(user);
        
        return TelegramClient.SendMessage($"{userInfo}\r\n\r\n" +
                                           "У вас нет токена для почты. Выполните команду /registermail");
    }

    private Task ReturnUnregisteredName(User user)
    {
        var userInfo = GetUserInfo(user);
        
        return TelegramClient.SendMessage($"{userInfo}\r\n\r\n" +
                                           "У вас не заданы данные о пользователе. Выполните команду /registeruser");
    }

    private Task ReturnUserInfo(User user)
    {
        var userInfo = GetUserInfo(user);
        
        return TelegramClient.SendMessage(userInfo);
    }

    private string GetUserInfo(User user)
    {
        var result = $"<strong>Имя:</strong> {user.Name ?? "не задано"}\r\n" +
                   $"<strong>Имя в Р.П.:</strong> {user.NameGenitive ?? "не задано"}\r\n" +
                   $"<strong>Должность:</strong> {user.JobTitle ?? "не задана"}\r\n" +
                   $"<strong>Должность в Р.П.:</strong> {user.JobTitleGenitive ?? "не задана"}\r\n" +
                   $"<strong>Почта:</strong> {user.Email ?? "не задана"}";

        return result;
    }
}