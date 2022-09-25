namespace Secretary.Panel.Models;

public class UserStatistic
{
    public int TotalUsers { get; set; }
    public int UserWithDocuments { get; set; }
    public int UserWithValidTokens { get; set; }
    public int UserWithNotifications { get; set; }
}