using Secretary.Documents.utils;

namespace Secretary.Documents.Tests.Utils;

public class InsertStringFormatterTests
{
    private InsertStringFormatter _formatter = new ();

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
    
    [Test]
    public void ShouldRemoveRedundantWhitespaces()
    {
        var result = _formatter.Format("one  two   three", extraPoint: false);
        
        Assert.That(result, Is.EqualTo("one two three"));
    }
    
    [Test]
    public void ShouldUppercaseFirstLetter()
    {
        var result = _formatter.Format("line", extraPoint: false, firstLetter: FirstLetter.Upper);
        
        Assert.That(result, Is.EqualTo("Line"));
    }
    
    [Test]
    public void ShouldUppercaseOneLetter()
    {
        var result = _formatter.Format("l", extraPoint: false, firstLetter: FirstLetter.Upper);
        
        Assert.That(result, Is.EqualTo("L"));
    }
    
    [Test]
    public void ShouldLowercaseFirstLetter()
    {
        var result = _formatter.Format("Line", extraPoint: false, firstLetter: FirstLetter.Lower);
        
        Assert.That(result, Is.EqualTo("line"));
    }
    
    [Test]
    public void ShouldLowercaseOneLetter()
    {
        var result = _formatter.Format("L", extraPoint: false, firstLetter: FirstLetter.Lower);
        
        Assert.That(result, Is.EqualTo("l"));
    }
}