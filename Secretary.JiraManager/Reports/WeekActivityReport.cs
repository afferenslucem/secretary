using Atlassian.Jira;
using Secretary.JiraManager.Data;

namespace Secretary.JiraManager.Reports;

public class WeekActivityReport
{ 
    public DateOnly WeekStart { get; }
    
    public IEnumerable<DayActivityReport> DayReports { get; private set; }
    public float TotalTime { get; private set; }

    public WeekActivityReport(IEnumerable<WorkData> workData, DateOnly weekStart)
    {
        WeekStart = weekStart;

        var reports = new List<DayActivityReport>();
        
        var reportDate = weekStart;
        
        for (var i = 0; i < 7; i++)
        {
            reports.Add(new DayActivityReport(workData, reportDate));
            reportDate = reportDate.AddDays(1);
        }

        DayReports = reports;
        
        TotalTime = DayReports.Aggregate(0f, (acc, item) => acc + item.TotalTime);
    }
}