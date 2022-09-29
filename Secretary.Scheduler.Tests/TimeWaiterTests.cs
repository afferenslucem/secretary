namespace Secretary.Scheduler.Tests;

public class TimeWaiterTests
{
    private TimeWaiter _timeWaiter = null!;
    
    [SetUp]
    public void Setup()
    {
        _timeWaiter = new TimeWaiter();
    }

    [Test]
    public void ShouldFireEvent()
    {
        _timeWaiter.TargetDate = DateTime.UtcNow.AddSeconds(-1);

        var fired = false;

        _timeWaiter.OnTime += () =>
        {
            fired = true;
            return Task.CompletedTask;
        };
        
        _timeWaiter.Check();
        
        Assert.That(fired, Is.True);
    }

    [Test]
    public void ShouldNotFireEvent()
    {
        _timeWaiter.TargetDate = DateTime.UtcNow.AddMinutes(1);

        var fired = false;

        _timeWaiter.OnTime += () =>
        {
            fired = true;
            return Task.CompletedTask;
        };
        
        _timeWaiter.Check();
        
        Assert.That(fired, Is.False);
    }
}