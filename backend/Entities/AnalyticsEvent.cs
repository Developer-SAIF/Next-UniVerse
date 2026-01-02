using System.Text.Json;

namespace backend.Entities;

public class AnalyticsEvent
{
    public long Id { get; set; }

    public long? UserId { get; set; }
    public User? User { get; set; }

    public string? EventType { get; set; }
    public JsonDocument? EventData { get; set; }
    public DateTime CreatedAt { get; set; }
}
