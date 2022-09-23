namespace Secretary.HealthCheck.Data;

public class BotHealthData
{
    public DateTime PingTime { get; set; }
    public DateTime DeployTime { get; set; }
    public long ReceivedMessages { get; set; }
    public string Version { get; set; } = null!;
}