using System.Text;
using Secretary.JiraManager.Reports;

namespace Secretary.Telegram.Commands.Formatters;

public class WeekReportFormatter
{
    public string GetHtmlView(WeekActivityReport report)
    {
        var message = new StringBuilder();

        message.Append($"{report.WeekStart:yyyy-MM-dd} - {report.WeekStart.AddDays(6):yyyy-MM-dd}\n\n");

        foreach (var dayReport in report.DayReports.Where(day => day.TotalTime > 0))
        {
            var dayView = new DayReportFormatter().GetHtmlView(dayReport);

            message.Append(dayView);
            message.Append("----------------------------------------------------------\n");
        }

        message.Append($"За неделю: {report.TotalTime:F}h\n");

        return message.ToString();
    }
}