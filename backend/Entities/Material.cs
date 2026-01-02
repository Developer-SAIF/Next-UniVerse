namespace backend.Entities;

public class Material
{
    public long Id { get; set; }

    public long ModuleId { get; set; }
    public Module Module { get; set; } = default!;

    public MaterialKind Kind { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? StorageUrl { get; set; }
    public string? MimeType { get; set; }
    public long? SizeBytes { get; set; }
    public bool RequiresAuth { get; set; }
    public int? Position { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }

    public VideoStream? VideoStream { get; set; }
}
