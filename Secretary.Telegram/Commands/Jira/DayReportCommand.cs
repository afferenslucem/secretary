using Secretary.Configuration;
using Secretary.JiraManager;
using Secretary.Logging;
using Secretary.Telegram.Commands.Abstractions;
using Secretary.Telegram.Commands.Formatters;
using Secretary.Telegram.MenuPrinters;
using Secretary.Utils;
using Serilog;

namespace Secretary.Telegram.Commands.Jira;

public class DayReportCommand: Command
{
    private readonly ILogger _logger = LogPoint.GetLogger<DayReportCommand>();
    
    public const string Key = "/daytimereport";

    public IJiraReporterFactory JiraReporterFactory;

    public DayReportCommand()
    {
        JiraReporterFactory = new JiraReporterFactory();
    }
    
    public override async Task Execute()
    {
        var argument = Context.UserMessage.CommandArgument;

        var date = argument != null
            ? DateOnly.ParseExact(argument, "yyyy-MM-dd")
            : DateUtils.DateEkb;

        var user = await UserStorage.GetUser();
        var reporter = JiraReporterFactory.Create(Config.Instance.JiraConfig.Host, user.JiraPersonalAccessToken);
        var report = await reporter.GetDayActivityReport(date);

        var view = new DayReportFormatter().GetHtmlView(report);

        var printer = new DayReportMenuPrinter(date, view);

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