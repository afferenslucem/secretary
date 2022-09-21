namespace Secreatry.HealthCheck.Data;

public class HealthData
{
    public BotHealthData BotHealthData { get; set; }
    public RefresherHealthData RefresherHealthData { get; set; }
    
    public HealthData()
    {
        RefresherHealthData = new RefresherHealthData();
        BotHealthData = new BotHealthData();
    }
}