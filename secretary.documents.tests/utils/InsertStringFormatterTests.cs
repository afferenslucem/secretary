using secretary.documents.utils;

namespace secretary.documents.tests.utils;

public class InsertStringFormatterTests
{
    private InsertStringFormatter _formatter = new InsertStringFormatter();

    [Test]
    public void ShouldAddPointIfLineDoesNotEndWithPoint()
    {
        var result = _formatter.Format("line");
        
        Assert.That(result, Is.EqualTo("line."));
    }
    
    [Test]
    public void ShouldSkipPointIfLineEndsWithPoint()
    {
        var result = _formatter.Format("line.");
        
        Assert.That(result, Is.EqualTo("line."));
    }
    
    [Test]
    public void ShouldSkipPointIfLineEndsWithQuestionMark()
    {
        var result = _formatter.Format("line?");
        
        Assert.That(result, Is.EqualTo("line?"));
    }
    
    [Test]
    public void ShouldSkipPointIfLineEndsWithExclamationPoint()
    {
        var result = _formatter.Format("line!");
        
        Assert.That(result, Is.EqualTo("line!"));
    }
}