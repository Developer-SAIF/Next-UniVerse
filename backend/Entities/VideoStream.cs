namespace backend.Entities;

public class VideoStream
{
    public long Id { get; set; }

    public long MaterialId { get; set; }
    public Material Material { get; set; } = default!;

    public string? Provider { get; set; }
    public string? StreamUrl { get; set; }
    public string? PlaybackPolicy { get; set; }
    public int? DurationSeconds { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
