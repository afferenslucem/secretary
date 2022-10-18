using Atlassian.Jira;
using Secretary.JiraManager.Data;
using Secretary.JiraManager.Reports;

namespace Secretary.JiraManager;

public class JiraReporter: IJiraReporter
{
    public IJiraConnector JiraConnector;

    public JiraReporter(string url, string token)
    {
        JiraConnector = new JiraConnector(url, token);
    }
    
    public async Task<DayActivityReport> GetDayActivityReport(DateOnly date)
    {
        var data = await GetWorkDataBetween(date, date.AddDays(1));

        var report = new DayActivityReport(data, date);

        return report;
    }
    
    public async Task<WeekActivityReport> GetWeekActivityReport(DateOnly weekStart)
    {
        var data = await GetWorkDataBetween(weekStart, weekStart.AddDays(7));

        var report = new WeekActivityReport(data, weekStart);

        return report;
    }

    public async Task<Page<IssueInfo>> GetMyInProgressIssues(int page = 1, int pageLength = 5)
    {
        var pageAnswer = await JiraConnector.GetMyInProgressIssues(page, pageLength);

        var data = pageAnswer.Select(issue => new IssueInfo(issue));

        return new Page<IssueInfo>(page, pageLength, pageAnswer.TotalItems, data);
    }

    public async Task<Page<IssueInfo>> GetMyToDoIssues(int page = 1, int pageLength = 5)
    {
        var pageAnswer = await JiraConnector.GetMyToDoIssues(page, pageLength);

        var data = pageAnswer.Select(issue => new IssueInfo(issue));

        return new Page<IssueInfo>(page, pageLength, pageAnswer.TotalItems, data);
    }

    public async Task<IssueInfo?> GetIssueInfo(string key)
    {
        var issue = await JiraConnector.GetIssue(key);

        if (issue == null) return null;
        
        var logs = issue.GetWorklogsAsync();
        var attachement = issue.GetAttachmentsAsync();
        var commnets = issue.GetCommentsAsync();

        await Task.WhenAll(logs, attachement, commnets);

        return new IssueInfo(issue, logs.Result, attachement.Result, commnets.Result);
    }

    public async Task<float> GetWorkingHoursForPeriod(DateOnly from, DateOnly to)
    {
        var data = await GetWorkDataBetween(from, to);

        var result = data.SelectMany(item => item.Worklogs)
            .Aggregate(0L, (acc, worklog) => acc + worklog.TimeSpentInSeconds);

        return result / 60f / 60f;
    }

    public async Task<string> GetMyUsername()
    {
        var user = await JiraConnector.GetMe();

        return user.Username;
    }

    private async Task<IEnumerable<WorkData>> GetWorkDataBetween(DateOnly from, DateOnly to)
    {
        var user = await JiraConnector.GetMe();

        var issues = await JiraConnector.GetWorkedIssuesBetween(from, to);
        
        var worklogsCollection = await Task.WhenAll(
            issues.Select(async issue =>
            {
                var worklogs = await issue.GetWorklogsAsync();
                worklogs = worklogs
                    .Where(worklog => worklog.Author == user.Username)
                    .Where(worklog => worklog.StartDate.HasValue)
                    .Where(worklog => worklog.StartDate >= from.ToDateTime(TimeOnly.MinValue))
                    .Where(worklog => worklog.StartDate < to.ToDateTime(TimeOnly.MinValue));

                return new WorkData(issue, worklogs);
            })
        );

        var result = worklogsCollection.Where(item => item.Worklogs.Count() > 0);

        return result;
    }

    public async Task LogTime(string key, string worklog)
    {
        var issue = await JiraConnector.GetIssue(key);

        await issue.AddWorklogAsync(worklog);
    }
}