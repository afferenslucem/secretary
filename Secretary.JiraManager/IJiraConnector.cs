using Atlassian.Jira;

namespace Secretary.JiraManager;

public interface IJiraConnector
{
    Task<JiraUser> GetMe();
    Task<IEnumerable<Issue>> GetWorkedIssuesBetween(DateOnly from, DateOnly to);
    Task<Issue?> GetIssue(string key);
    Task<IPagedQueryResult<Issue>> GetMyInProgressIssues(int page, int pageLength);
    Task<IPagedQueryResult<Issue>> GetMyToDoIssues(int page, int pageLength);
}