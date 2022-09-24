namespace Secretary.HealthCheck.Data;

public class RefresherHealthData
{
    public DateTime PingTime { get; set; }
    public DateTime DeployTime { get; set; }
    public DateTime NextRefreshDate { get; set; }
    public string Version { get; set; }
}