namespace Secretary.Telegram.Tests;

public class UserMessageTests
{
    [Test]
    public void ShouldSetIsCommandFlagTrue()
    {
        var userMessage = new UserMessage(2517, "pushkin", "/timeoff", 12);
        
        Assert.That(userMessage.IsCommand, Is.True);
    }
    
    [Test]
    public void ShouldSetIsCommandFlagFalse()
    {
        var userMessage = new UserMessage(2517, "pushkin", "Hello", 12);
        
        Assert.That(userMessage.IsCommand, Is.False);
    }
    
    [Test]
    public void ShouldSetCommandWithCommandString()
    {
        var userMessage = new UserMessage(2517, "pushkin", "/print", 12);
        
        Assert.That(userMessage.CommandText, Is.EqualTo("/print"));
    }
    
    [Test]
    public void ShouldSetCommandWithCommandStringWithArgs()
    {
        var userMessage = new UserMessage(2517, "pushkin", "/print hello", 12);
        
        Assert.That(userMessage.CommandText, Is.EqualTo("/print"));
    }
    
    [Test]
    public void ShouldSetCommandArgsNullWithCommandString()
    {
        var userMessage = new UserMessage(2517, "pushkin", "/print", 12);
        
        Assert.That(userMessage.CommandArgument, Is.Null);
    }
    
    [Test]
    public void ShouldSetHasArgumentsFalseWithCommandString()
    {
        var userMessage = new UserMessage(2517, "pushkin", "/print", 12);
        
        Assert.That(userMessage.HasArguments, Is.False);
    }
    
    [Test]
    public void ShouldSetCommandArgsWithCommandArgsString()
    {
        var userMessage = new UserMessage(2517, "pushkin", "/print hello", 12);
        
        Assert.That(userMessage.CommandArgument, Is.EqualTo("hello"));
    }
    
    [Test]
    public void ShouldSetHasArgumentsTrueWithCommandArgsString()
    {
        var userMessage = new UserMessage(2517, "pushkin", "/print hello", 12);
        
        Assert.That(userMessage.HasArguments, Is.True);
    }
    
    [Test]
    public void ShouldSetIsCallbackFlagTrue()
    {
        var userMessage = new UserMessage(2517, "pushkin", "/print hello", 12);
        
        Assert.That(userMessage.IsCallback, Is.True);
    }
    
    [Test]
    public void ShouldSetIsCallbackFlagFalse()
    {
        var userMessage = new UserMessage(2517, "pushkin", "/print hello", null);
        
        Assert.That(userMessage.IsCallback, Is.False);
    }
}