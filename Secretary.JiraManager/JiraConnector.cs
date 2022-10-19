using System.Diagnostics;
using Atlassian.Jira;
using Secretary.JiraManager.Connection;
using Secretary.Logging;
using Serilog;

namespace Secretary.JiraManager;

public class JiraConnector: IJiraConnector
{
    private readonly ILogger _logger = LogPoint.GetLogger<JiraConnector>();
    private readonly string _url;
    private readonly string _token;
    private readonly Jira _client;

    public JiraConnector(string url, string token)
    {
        _url = url;
        _token = token;
        
        _client = CreateRestClient();
    }

    private Jira CreateRestClient()
    {
        var settings = new JiraRestClientSettings();
        return Jira.CreateRestClient(new JiraBearerRestClient(_url, _token), settings.Cache);
    }
    
    public async Task<IPagedQueryResult<Issue>> GetMyInProgressIssues(int page, int pageLength)
    {
        var sw = new Stopwatch();
        sw.Start();
        var result = await _client.Issues.GetIssuesFromJqlAsync(
            "assignee = currentUser() and status not in (Done, Resolved, Open, Reopened) ORDER BY updated desc",
            pageLength,
            (page - 1) * pageLength
        );
        sw.Stop();

        _logger.Debug($"Got {result.Count()} issues in progress for page {page} length {pageLength} " +
                      $"in {sw.ElapsedMilliseconds / 1000f}s");
        
        return result;
    }
    
    public async Task<IPagedQueryResult<Issue>> GetMyToDoIssues(int page, int pageLength)
    {        
        var sw = new Stopwatch();
        sw.Start();
        var result = await _client.Issues.GetIssuesFromJqlAsync("assignee = currentUser() and status in (Open, Reopened, \"To Do\")",
            pageLength,
            (page - 1) * pageLength);
        sw.Stop();

        _logger.Debug($"Got {result.Count()} issues to do for page {page} length {pageLength} " +
                      $"in {sw.ElapsedMilliseconds / 1000f}s");
        
        return result;
    }

    public Task<Issue?> GetIssue(string key)
    {
        return Task.Run(() => _client.Issues.Queryable.FirstOrDefault(issue => issue.Key == key));
    }

    public async Task<IEnumerable<Issue>> GetWorkedIssuesBetween(DateOnly from, DateOnly to)
    {
        var jql = $"worklogAuthor = currentUser() and updated >= {from.ToString("yyyy-MM-dd")} and " +
                  $"created < {to.ToString("yyyy-MM-dd")} order by key";
        
        var sw = new Stopwatch();
        sw.Start();
        var issues = await _client.Issues.GetIssuesFromJqlAsync(jql, Int32.MaxValue);
        sw.Stop();

        _logger.Debug($"Got worked {issues.Count()} issues " +
                      $"for period {from:yyyy-MM-dd} - {to:yyyy-MM-dd} " +
                      $"in {sw.ElapsedMilliseconds / 1000f}s");

        return issues;
    }

    public Task<JiraUser> GetMe()
    {
        return _client.Users.GetMyselfAsync();
    }
    
    public Task<IEnumerable<IssueStatus>> GetAllStatuses()
    {
        return _client.Statuses.GetStatusesAsync();
    }
    
    public Task<IEnumerable<IssuePriority>> GetAllPriorities()
    {
        return _client.Priorities.GetPrioritiesAsync();
    }
}