namespace backend.Entities;

public class Notification
{
    public long Id { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = default!;

    public NotificationType Type { get; set; }
    public string? Message { get; set; }
    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
