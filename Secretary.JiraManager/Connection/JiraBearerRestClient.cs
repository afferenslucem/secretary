using Atlassian.Jira;
using Atlassian.Jira.Remote;

namespace Secretary.JiraManager.Connection;

public class JiraBearerRestClient: JiraRestClient
{
    public JiraBearerRestClient(string url, string token, JiraRestClientSettings settings = null) : base(url, new HttpBearerAuthenticator(token), settings)
    {
    }
}