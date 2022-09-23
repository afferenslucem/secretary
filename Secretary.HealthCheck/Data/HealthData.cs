namespace Secretary.HealthCheck.Data;

public class HealthData
{
    public BotHealthData? BotHealthData { get; set; }
    public ReminderHealthData? ReminderHealthData { get; set; }
    public RefresherHealthData? RefresherHealthData { get; set; }
}