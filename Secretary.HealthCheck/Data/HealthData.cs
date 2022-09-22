using Secreatry.HealthCheck.Data;

namespace Secretary.HealthCheck.Data;

public class HealthData
{
    public BotHealthData BotHealthData { get; set; }
    public ReminderHealthData ReminderHealthData { get; set; }
    public RefresherHealthData RefresherHealthData { get; set; }
    
    public HealthData()
    {
        RefresherHealthData = new RefresherHealthData();
        ReminderHealthData = new ReminderHealthData();
        BotHealthData = new BotHealthData();
    }
}