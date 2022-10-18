using Atlassian.Jira;

namespace Secretary.JiraManager.Data;

public class IssueInfo
{
    public string Key = null!;
    public string Assignee = null!;
    public string Priority = null!;
    public string Status = null!;
    public string Type = null!;
    public string Summary = null!;
    public string Description = null!;

    public IEnumerable<Worklog>? Worklogs;
    public IEnumerable<Attachment>? Attachment;
    public IEnumerable<Comment>? Comments;

    public IssueInfo(Issue issue, IEnumerable<Worklog>? worklogs = null, IEnumerable<Attachment>? attachment = null, IEnumerable<Comment>? comments = null)
    {
        Key = issue.Key.Value;
        Assignee = issue.Assignee;
        Priority = issue.Priority.Name;
        Status = issue.Status.Name;
        Type = issue.Type.Name;
        Summary = issue.Summary;
        Description = issue.Description;

        Worklogs = worklogs;
        Attachment = attachment;
        Comments = comments;
    }
    
    public IssueInfo() { }
}