using Secretary.Configuration;

namespace Secretary.JiraManager;

public class JiraReporterFactory: IJiraReporterFactory
{
    public IJiraReporter Create(string url, string pat) => new JiraReporter(url, pat);
}