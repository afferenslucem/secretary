using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Jira;
using Secretary.Telegram.Utils;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.MenuPrinters;

public class DayReportMenuPrinter: MenuPrinter
{
    public readonly DateOnly Date;
    public readonly string Html;
    
    public DayReportMenuPrinter(DateOnly date, string html)
    {
        Date = date;
        Html = html;
    }
    
    protected override Task<Menu> CreateMenu(CommandContext context)
    {
        var previous =
            InlineKeyboardButton.WithCallbackData("◀",$"{DayReportCommand.Key} {Date.AddDays(-1):yyyy-MM-dd}");
        
        var next = Date < DateUtils.DateEKB
            ? InlineKeyboardButton.WithCallbackData("▶", $"{DayReportCommand.Key} {Date.AddDays(1):yyyy-MM-dd}")
            : InlineKeyboardButton.WithCallbackData("🚫", $"{EmptyCommand.Key}");
        
        var menuButtons = new[]
        {
            new[] { previous, next },
        };
        
        var result = new Menu(Html, menuButtons);
        
        return Task.FromResult(result);
    }
}