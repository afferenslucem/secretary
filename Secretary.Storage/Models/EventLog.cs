using System.ComponentModel.DataAnnotations;

namespace Secretary.Storage.Models;

public class EventLog
{
    [Key]
    public long Id { get; set; }

    public string EventType { get; set; } = null!;

    public DateTime Time { get; set; }

    public string? Description { get; set; } = null!;
    
    public User? User { get; set; }
    public long? UserChatId { get; set; }
}