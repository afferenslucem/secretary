using Atlassian.Jira;
using Secretary.JiraManager.Data;
using Secretary.JiraManager.Reports;

namespace Secretary.JiraManager;

public interface IJiraReporter
{
    Task<DayActivityReport> GetDayActivityReport(DateOnly date);

    Task<WeekActivityReport> GetWeekActivityReport(DateOnly weekStart);

    Task<Page<IssueInfo>> GetMyInProgressIssues(int page = 1, int pageLength = 5);

    Task<Page<IssueInfo>> GetMyToDoIssues(int page = 1, int pageLength = 5);

    Task<IssueInfo?> GetIssueInfo(string key);

    Task<float> GetWorkingHoursForPeriod(DateOnly from, DateOnly to);
    Task<string> GetMyUsername();

    Task LogTime(string issueKey, string worklog);
}