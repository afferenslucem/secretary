using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Jira;
using Secretary.Telegram.Commands.Jira.Lists;
using Secretary.Telegram.Commands.Menus;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.MenuPrinters;

public class IssueListsMenuPrinter: MenuPrinter
{
    protected override Task<Menu> CreateMenu(CommandContext context)
    {
        var menuButtons = new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("To Do", OpenIssuesListCommand.Key) },
            new[] { InlineKeyboardButton.WithCallbackData("In Progress", "/progressissues") },
            new[] { InlineKeyboardButton.WithCallbackData( "Назад", JiraMenuCommand.Key) },
        };

        var result = new Menu("Выберете список задач", menuButtons);

        return Task.FromResult(result);
    }
}