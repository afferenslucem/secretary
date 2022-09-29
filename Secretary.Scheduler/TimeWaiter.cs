namespace Secretary.Scheduler;

public class TimeWaiter
{
    public delegate Task AsyncAction();
    
    public DateTime TargetDate { get; set; }

    public event AsyncAction? OnTime;

    public void Check()
    {
        if (DateTime.UtcNow >= TargetDate)
        {
            OnTime?.Invoke();
        }
    }
}