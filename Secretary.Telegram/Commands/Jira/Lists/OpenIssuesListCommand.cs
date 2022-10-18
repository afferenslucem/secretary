using Secretary.Configuration;
using Secretary.JiraManager;
using Secretary.JiraManager.Data;
using Secretary.Telegram.Commands.Caches.Jira;
using Secretary.Telegram.MenuPrinters;
using Telegram.Bot.Types;

namespace Secretary.Telegram.Commands.Jira.Lists;

public class OpenIssuesListCommand: IssuesListCommand<OpenIssuesListNavigationCache>
{
    public const string Key = "/todoissues";
    
    public OpenIssuesListCommand(): base(new JiraReporterFactory())
    {
    }

    public override async Task<Page<IssueInfo>> GetIssues(int page = 1, int pageLength = 5)
    {
        var user = await UserStorage.GetUser();

        var reporter = JiraReporterFactory.Create(Config.Instance.JiraConfig.Host, user.JiraPersonalAccessToken);
        
        return await reporter.GetMyToDoIssues(page, pageLength);
    }
    
    public override Task<Message> PrintNavigation(int page, int pageLength, int totalIssues)
    {
        return new OpenIssuesNavigationMenuPrinter(page, 5, totalIssues).Print(Context);
    }

    public override Task ReprintNavigation(int page, int pageLength, int totalIssues, int messageId)
    {
        return new OpenIssuesNavigationMenuPrinter(page, 5, totalIssues).Reprint(Context);
    }

    public override string GetListHeader()
    {
        return "📌 <b>Список открытых задач</b> 📌";
    }
}