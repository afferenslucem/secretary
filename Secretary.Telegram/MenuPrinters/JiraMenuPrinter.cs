using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Jira;
using Secretary.Telegram.Commands.Jira.Lists;
using Secretary.Telegram.Commands.Menus;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.MenuPrinters;

public class JiraMenuPrinter: MenuPrinter
{
    protected override async Task<Menu> CreateMenu(CommandContext context)
    {
        var user = await context.UserStorage.GetUser(context.ChatId);

        var remindLogTime = user.RemindLogTime
            ? "Выключить напоминания о логгировани"
            : "Включить напоминания о логгировани";
        
        var menuButtons = new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Открытые Задачи", OpenIssuesListCommand.Key) },
            new[] { InlineKeyboardButton.WithCallbackData("Задачи В Прогрессе", ProgressIssuesListCommand.Key) },
            new[] { InlineKeyboardButton.WithCallbackData("Отчет За День", DayReportCommand.Key) },
            new[] { InlineKeyboardButton.WithCallbackData("Отчет За Неделю", WeekReportCommand.Key) },
            new[] { InlineKeyboardButton.WithCallbackData( remindLogTime, "/remindlogtime") },
        };

        return new Menu("Меню команд для Jira", menuButtons);
    }
}