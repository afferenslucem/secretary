using Atlassian.Jira;

namespace Secretary.JiraManager.Data;

public class WorkData
{
    public Issue Issue;
    public IEnumerable<Worklog> Worklogs;

    public WorkData(Issue issue, IEnumerable<Worklog> worklogs)
    {
        Issue = issue;
        Worklogs = worklogs;
    }
}