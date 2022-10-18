using System.Globalization;
using Secretary.JiraManager.Data;
using Secretary.JiraManager.Reports;
using Secretary.Telegram.Commands.Formatters;

namespace Secretary.Telegram.Tests.Commands.Formatters;

public class DayReportFormatterTests
{
    [SetUp]
    public void Setup()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }
    
    [Test]
    public void ShouldContainsDate()
    {
        var report = new DayActivityReport(new WorkData[0], new DateOnly(2022, 10, 18));
    
        var result = new DayReportFormatter().GetHtmlView(report);

        var contains = result.Contains("2022.10.18");
        
        Assert.That(contains, Is.True);
    }
    
    [Test]
    public void ShouldContainsDayOfWeek()
    {
        var report = new DayActivityReport(new WorkData[0], new DateOnly(2022, 10, 18));
    
        var result = new DayReportFormatter().GetHtmlView(report);

        var contains = result.Contains("Tuesday");
        
        Assert.That(contains, Is.True);
    }
    
    [Test]
    public void ShouldContainsIssueInfo()
    {
        var report = new DayActivityReport(new WorkData[0], new DateOnly(2022, 10, 18));

        var issue = new IssueInfo();

        issue.Key = "ONG-1234";
        issue.Summary = "Опечатка в третьей строфе";
        
        report.Issues = new[] { issue };
        report.Worklogs[issue.Key] = 3;
        
        var result = new DayReportFormatter().GetHtmlView(report);

        var contains = result.Contains("3.00h - <a href=\"https://jira.pushkin.ru/browse/ONG-1234\">ONG-1234</a> Опечатка в третьей строфе");
        
        Assert.That(contains, Is.True);
    }
    
    [Test]
    public void ShouldContainsTotalTime()
    {
        var report = new DayActivityReport(new WorkData[0], new DateOnly(2022, 10, 18));

        report.TotalTime = 5.56f;
        
        var result = new DayReportFormatter().GetHtmlView(report);

        var contains = result.Contains("Всего времени: 5.56h");
        
        Assert.That(contains, Is.True);
    }
}