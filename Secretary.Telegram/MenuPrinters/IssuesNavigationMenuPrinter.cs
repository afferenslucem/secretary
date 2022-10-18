using Secretary.Telegram.Commands;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Jira.Lists;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.MenuPrinters;

public class IssuesNavigationMenuPrinter: MenuPrinter
{
    public int Page { get; init; }
    public int PageLength { get; init; }
    public int TotalIssues { get; init; }
    public int TotalPages { get; init; }
    public bool IsLastPage { get; init; }
    public bool IsFirstPage { get; init; }

    private string _commandKey;

    public IssuesNavigationMenuPrinter(string commandKey, int page, int pageLength, int totalIssues)
    {
        _commandKey = commandKey;
        Page = page;
        PageLength = pageLength;
        TotalIssues = totalIssues;

        var temp = TotalIssues % pageLength;
        TotalPages = TotalIssues / PageLength + (temp > 0 ? 1 : 0);

        IsLastPage = page == TotalPages;
        IsFirstPage = page == 1;
    }

    protected override Task<Menu> CreateMenu(CommandContext context)
    {
        var beginButton = !IsFirstPage
            ? InlineKeyboardButton.WithCallbackData("⏪", $"{_commandKey} 1")
            : InlineKeyboardButton.WithCallbackData("🚫", $"{EmptyCommand.Key}");
        
        var goBackButton = !IsFirstPage
            ? InlineKeyboardButton.WithCallbackData("◀", $"{_commandKey} {Page - 1}")
            : InlineKeyboardButton.WithCallbackData("🚫", $"{EmptyCommand.Key}");

        var refresh = InlineKeyboardButton.WithCallbackData("🔄", $"{_commandKey} {Page}");
        
        var goForwardButton = !IsLastPage
            ? InlineKeyboardButton.WithCallbackData("▶", $"{_commandKey} {Page + 1}")
            : InlineKeyboardButton.WithCallbackData("🚫", $"{EmptyCommand.Key}");
        
        var endButton = !IsLastPage
            ? InlineKeyboardButton.WithCallbackData("⏩", $"{_commandKey} {TotalPages}")
            : InlineKeyboardButton.WithCallbackData("🚫", $"{EmptyCommand.Key}");
        
        var menuButtons = new[]
        {
            new[] { beginButton, goBackButton, goForwardButton, endButton },
        };

        var text = 
            $"Страница: {Page}\n" +
            $"Всего страниц: {TotalPages}\n" +
            $"Всего задач: {TotalIssues}";

        var result = new Menu(text, menuButtons);

        return Task.FromResult(result);
    }
}