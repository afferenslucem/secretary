namespace Secretary.HealthCheck.Data;

public class ReminderHealthData
{
    public DateTime PingTime { get; set; }
    public DateTime NextNotifyDate { get; set; }
}