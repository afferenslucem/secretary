using Atlassian.Jira;
using Secretary.JiraManager.Data;

namespace Secretary.JiraManager.Reports;

public class DayActivityReport
{ 
    public DateOnly Date { get; }
    public IEnumerable<IssueInfo> Issues { get; set; }
    public Dictionary<string, float> Worklogs { get; init; }
    public float TotalTime { get; set; }

    public DayActivityReport(IEnumerable<WorkData> workData, DateOnly date)
    {
        Date = date;
        Worklogs = new();

        var actualData = workData.Where(
            data => data.Worklogs.Count(worklog => worklog.StartDate == date.ToDateTime(TimeOnly.MinValue)) > 0
        );
        
        foreach (var tuple in actualData)
        {
            var key = tuple.Issue.Key.Value;
            var timeSeconds = tuple.Worklogs
                .Where(worklog => DateOnly.FromDateTime(worklog.StartDate!.Value) == date)
                .Aggregate(0L, (time, worklog) => time + worklog.TimeSpentInSeconds);

            var timeHours = timeSeconds / 60f / 60f;
            
            Worklogs.Add(key, timeHours);
        }

        Issues = actualData.Select(item => new IssueInfo(item.Issue));
        
        TotalTime = Worklogs.Values.Aggregate(0f, (acc, item) => acc + item);
    }
}