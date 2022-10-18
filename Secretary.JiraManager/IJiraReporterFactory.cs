namespace Secretary.JiraManager;

public interface IJiraReporterFactory
{
    IJiraReporter Create(string url, string pat);
}