using Secretary.Logging;
using Secretary.Storage.Models;
using Serilog;

namespace Secretary.Telegram.Commands;

public class RemindLogTimeCommand: Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<RemindLogTimeCommand>();
    
    public const string Key = "/remindlogtime";
    
    public override async Task Execute()
    {
        _logger.Information($"{ChatId}: Switching RemindLogTime");

        var user = await UserStorage.GetUser();

        user ??= new User { ChatId = ChatId };

        user.RemindLogTime = !user.RemindLogTime;

        await UserStorage.SetUser(user);

        if (user.RemindLogTime)
        {
            await TelegramClient.SendMessage("Напоминания о логгировании времени включены.\n\n" + 
                                             "Чтобы их выключить отправьте эту команду еще раз");
        }
        else
        {
            await TelegramClient.SendMessage("Напоминания о логгировании времени выключены.\n\n" + 
                                             "Чтобы их включить отправьте эту команду еще раз");
        }
    }
}