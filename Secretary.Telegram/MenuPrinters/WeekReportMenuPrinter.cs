using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Jira;
using Secretary.Utils;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.MenuPrinters;

public class WeekReportMenuPrinter: MenuPrinter
{
    public readonly DateOnly Date;
    public readonly string Html;
    
    public WeekReportMenuPrinter(DateOnly date, string html)
    {
        Date = date;
        Html = html;
    }
    
    protected override Task<Menu> CreateMenu(CommandContext context)
    {
        var previous =
            InlineKeyboardButton.WithCallbackData("◀",$"{WeekReportCommand.Key} {Date.AddDays(-7):yyyy-MM-dd}");

        var weekStart = DateUtils.GetStartOfWeek(DateUtils.DateEkb);
        
        var next = Date < weekStart
            ? InlineKeyboardButton.WithCallbackData("▶", $"{WeekReportCommand.Key} {Date.AddDays(7):yyyy-MM-dd}")
            : InlineKeyboardButton.WithCallbackData("🚫", $"{EmptyCommand.Key}");
        
        var menuButtons = new[]
        {
            new[] { previous, next },
        };
        
        var result = new Menu(Html, menuButtons);
        
        return Task.FromResult(result);
    }
}