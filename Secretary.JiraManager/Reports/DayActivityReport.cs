using Secretary.JiraManager.Data;
using Secretary.Logging;
using Secretary.Utils;
using Serilog;

namespace Secretary.JiraManager.Reports;

public class DayActivityReport
{ 
    private readonly ILogger _logger = LogPoint.GetLogger<WeekActivityReport>();
    public DateOnly Date { get; }
    public IEnumerable<IssueInfo> Issues { get; set; }
    public Dictionary<string, float> Worklogs { get; init; }
    public float TotalTime { get; set; }

    public DayActivityReport(IEnumerable<WorkData> workData, DateOnly date)
    {
        _logger.Debug($"Creating report for {date:yyyy-MM-dd}");
        
        Date = date;
        Worklogs = new();

        var dayStart = date.ToDateTime(TimeOnly.MinValue);
        var dayEnd = date.ToDateTime(TimeOnly.MaxValue);
        
        var actualData = workData.Where(
            data => data.Worklogs.Count(worklog => DateUtils.ConvertToEkbTime(worklog.StartDate) >= dayStart && DateUtils.ConvertToEkbTime(worklog.StartDate) <= dayEnd) > 0
        );
        
        _logger.Debug($"Found {actualData.Count()} actual items for {date:yyyy-MM-dd}");
        
        foreach (var tuple in actualData)
        {
            var key = tuple.Issue.Key.Value;
            var timeSeconds = tuple.Worklogs
                .Where(worklog => DateUtils.ConvertToEkbTime(worklog.StartDate) >= dayStart && DateUtils.ConvertToEkbTime(worklog.StartDate) <= dayEnd)
                .Aggregate(0L, (time, worklog) => time + worklog.TimeSpentInSeconds);

            var timeHours = timeSeconds / 60f / 60f;
            
            Worklogs.Add(key, timeHours);
        }

        Issues = actualData.Select(item => new IssueInfo(item.Issue));
        
        TotalTime = Worklogs.Values.Aggregate(0f, (acc, item) => acc + item);
    }
}