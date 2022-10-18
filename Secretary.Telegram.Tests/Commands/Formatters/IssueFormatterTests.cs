using Secretary.JiraManager.Data;
using Secretary.Telegram.Commands.Formatters;

namespace Secretary.Telegram.Tests.Commands.Formatters;

public class IssueFormatterTests
{
    [Test]
    public void ShouldReturnHammerForTaskType()
    {
        var result = new IssueFormatter().FormatType("Task");
        
        Assert.That(result, Is.EqualTo("🔨"));
    }

    [Test]
    public void ShouldReturnCheckForSubtaskType()
    {
        var result = new IssueFormatter().FormatType("Sub-task");
        
        Assert.That(result, Is.EqualTo("✅"));
    }
    
    [Test]
    public void ShouldReturnBugForBugType()
    {
        var result = new IssueFormatter().FormatType("Bug");
        
        Assert.That(result, Is.EqualTo("🐛"));
    }
    
    [Test]
    public void ShouldReturnStarsForFeatureType()
    {
        var result = new IssueFormatter().FormatType("New Feature");
        
        Assert.That(result, Is.EqualTo("✨"));
    }
    
    [Test]
    public void ShouldReturnUpChartForImprovementType()
    {
        var result = new IssueFormatter().FormatType("Improvement");
        
        Assert.That(result, Is.EqualTo("📈"));
    }
    
    [Test]
    public void ShouldReturnCircleForTrivialPriority()
    {
        var result = new IssueFormatter().FormatPriority("Trivial");
        
        Assert.That(result, Is.EqualTo("⭕"));
    }

    [Test]
    public void ShouldReturnDownArrowForMinorPriority()
    {
        var result = new IssueFormatter().FormatPriority("Minor");
        
        Assert.That(result, Is.EqualTo("⬇"));
    }
    
    [Test]
    public void ShouldReturnUpArrowForMajorPriority()
    {
        var result = new IssueFormatter().FormatPriority("Major");
        
        Assert.That(result, Is.EqualTo("⬆"));
    }
    
    [Test]
    public void ShouldReturnFireForCriticalPriority()
    {
        var result = new IssueFormatter().FormatPriority("Critical");
        
        Assert.That(result, Is.EqualTo("🔥"));
    }
    
    [Test]
    public void ShouldReturnNoEntryForBlockerPriority()
    {
        var result = new IssueFormatter().FormatPriority("Blocker");
        
        Assert.That(result, Is.EqualTo("⛔"));
    }
    
    [Test]
    public void ShouldReturnTextView()
    {
        var issueInfo = new IssueInfo();

        issueInfo.Key = "ONG-123";
        issueInfo.Type = "Bug";
        issueInfo.Status = "In Progress";
        issueInfo.Summary = "Опечатка в названии";
        issueInfo.Priority = "Critical";
        
        var result = new IssueFormatter().GetData(issueInfo);
        
        Assert.That(result.Text, Is.EqualTo("🔥 🐛 <a href=\"https://jira.pushkin.ru/browse/ONG-123\">ONG-123</a> | <b>In Progress</b>\nОпечатка в названии"));
    }
    
    [Test]
    public void ShouldReturnLogTimeButton()
    {
        var issueInfo = new IssueInfo();
    
        issueInfo.Key = "ONG-123";
        issueInfo.Summary = "Опечатка в названии";
        issueInfo.Type = "Bug";
        issueInfo.Priority = "Critical";
        
        var result = new IssueFormatter().GetData(issueInfo);
        
        Assert.That(result.Buttons[0][0].Text, Is.EqualTo("Log Time"));
        Assert.That(result.Buttons[0][0].CallbackData, Is.EqualTo("/logtime ONG-123"));
    }
}