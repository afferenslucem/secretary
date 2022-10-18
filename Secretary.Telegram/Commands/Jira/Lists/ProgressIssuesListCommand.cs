using Secretary.Configuration;
using Secretary.JiraManager;
using Secretary.JiraManager.Data;
using Secretary.Telegram.Commands.Caches.Jira;
using Secretary.Telegram.MenuPrinters;
using Telegram.Bot.Types;

namespace Secretary.Telegram.Commands.Jira.Lists;

public class ProgressIssuesListCommand: IssuesListCommand<ProgressIssuesListNavigationCache>
{
    public const string Key = "/progressissues";

    public ProgressIssuesListCommand(): base(new JiraReporterFactory())
    {
    }

    public override async Task<Page<IssueInfo>> GetIssues(int page = 1, int pageLength = 5)
    {
        var user = await UserStorage.GetUser();

        var reporter = JiraReporterFactory.Create(Config.Instance.JiraConfig.Host, user.JiraPersonalAccessToken);
        
        return await reporter.GetMyInProgressIssues(page, pageLength);
    }

    public override Task<Message> PrintNavigation(int page, int pageLength, int totalIssues)
    {
        return new ProgressIssuesNavigationMenuPrinter(1, 5, totalIssues).Print(Context);
    }

    public override Task ReprintNavigation(int page, int pageLength, int totalIssues, int messageId)
    {
        return new ProgressIssuesNavigationMenuPrinter(1, 5, totalIssues).Reprint(Context);
    }

    public override string GetListHeader()
    {
        return "🏃 <b>Список задач в прогрессе</b> 🏃";
    }
}