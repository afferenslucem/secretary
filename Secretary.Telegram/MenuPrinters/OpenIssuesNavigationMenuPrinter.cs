using Secretary.Telegram.Commands.Jira.Lists;

namespace Secretary.Telegram.MenuPrinters;

public class OpenIssuesNavigationMenuPrinter: IssuesNavigationMenuPrinter
{
    public OpenIssuesNavigationMenuPrinter(int page, int pageLength, int totalIssues): base(OpenIssuesListCommand.Key, page, pageLength, totalIssues)
    {
    }
}