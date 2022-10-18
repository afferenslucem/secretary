using System.Text;
using Secretary.JiraManager.Reports;

namespace Secretary.Telegram.Commands.Formatters;

public class DayReportFormatter
{
    public string GetHtmlView(DayActivityReport report)
    {
        var message = new StringBuilder();

        message.Append($"<u>{report.Date:yyyy.MM.dd} {report.Date.DayOfWeek.ToString()}</u>\n");
        
        foreach (var issue in report.Issues)
        {
            var time = report.Worklogs[issue.Key];

            var summary = issue.Summary.Length > 35 ? issue.Summary.Substring(0, 35) + "…" : issue.Summary;
            var key = issue.Key;
            
            message.Append($"{time:F}h - <a href=\"{IssueFormatter.GetIssueLink(issue.Key)}\">{key}</a> {summary}\n");
        }
        message.Append($"<b>Всего времени: {report.TotalTime:F}h</b>\n");

        return message.ToString();
    }
}