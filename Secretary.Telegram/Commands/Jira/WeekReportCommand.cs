using System.Text;
using Secretary.Configuration;
using Secretary.JiraManager;
using Secretary.JiraManager.Data;
using Secretary.JiraManager.Reports;
using Secretary.Logging;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Formatters;
using Secretary.Telegram.MenuPrinters;
using Secretary.Telegram.Utils;
using Serilog;

namespace Secretary.Telegram.Commands.Jira;

public class WeekReportCommand: Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<WeekReportCommand>();
    
    public const string Key = "/weektimereport";

    public IJiraReporterFactory JiraReporterFactory;

    public WeekReportCommand()
    {
        JiraReporterFactory = new JiraReporterFactory();
    }
    
    public override async Task Execute()
    {
        var argument = Context.UserMessage.CommandArgument;

        var date = argument != null
            ? DateOnly.ParseExact(argument, "yyyy-MM-dd")
            : DateUtils.GetStartOfWeek(DateUtils.DateEKB);
        
        var user = await UserStorage.GetUser();
        var reporter = JiraReporterFactory.Create(Config.Instance.JiraConfig.Host, user.JiraPersonalAccessToken);
        var report = await reporter.GetWeekActivityReport(date);

        var view = new WeekReportFormatter().GetHtmlView(report);

        var printer = new WeekReportMenuPrinter(date, view);

        if (argument == null)
        {
            await printer.Print(Context);
        }
        else
        {
            await printer.Reprint(Context);
        }
    }
}