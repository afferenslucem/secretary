using Secretary.Telegram.Commands.Jira.Lists;

namespace Secretary.Telegram.MenuPrinters;

public class ProgressIssuesNavigationMenuPrinter: IssuesNavigationMenuPrinter
{  
    public ProgressIssuesNavigationMenuPrinter(int page, int pageLength, int totalIssues): base(ProgressIssuesListCommand.Key, page, pageLength, totalIssues)
    {
    }
}