using Secretary.Configuration;
using Secretary.JiraManager.Data;
using Secretary.Telegram.Commands.Jira.LogTime;
using Secretary.Telegram.MenuPrinters;
using Telegram.Bot.Types.ReplyMarkups;

namespace Secretary.Telegram.Commands.Formatters;

public class IssueFormatter
{
    public static string GetIssueLink(string issueKey)
    {
        return $"{Config.Instance.JiraConfig.Host}/browse/{issueKey}";
    }
    
    public Menu GetData(IssueInfo issue)
    {
        var type = FormatType(issue.Type);
        var priority = FormatPriority(issue.Priority);

        return new Menu(
            $"{priority} {type} <a href=\"{GetIssueLink(issue.Key)}\">{issue.Key}</a> | <b>{issue.Status}</b>\n{issue.Summary}",
            new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Log Time", $"{LogTimeCommand.Key} {issue.Key}")
                }
            }
        );
    }

    public string FormatType(string type)
    {
        switch (type)
        {
            case "Task": return char.ConvertFromUtf32(0x1F528);
            case "Sub-task": return char.ConvertFromUtf32(0x2705);
            case "Bug": return char.ConvertFromUtf32(0x1F41B);
            case "New Feature": return char.ConvertFromUtf32(0x2728);
            case "Improvement": return char.ConvertFromUtf32(0x1F4C8);
            default: return "";
        }
    }

    public string FormatPriority(string priority)
    {
        switch (priority)
        {
            case "Trivial": return char.ConvertFromUtf32(0x2B55);
            case "Minor": return char.ConvertFromUtf32(0x2B07);
            case "Major": return char.ConvertFromUtf32(0x2B06);
            case "Critical": return char.ConvertFromUtf32(0x1F525);
            case "Blocker": return char.ConvertFromUtf32(0x26D4);
            default: return "";
        }
    }
}