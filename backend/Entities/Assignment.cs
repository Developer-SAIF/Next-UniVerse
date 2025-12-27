namespace backend.Entities;

public class Assignment
{
    public long Id { get; set; }

    public long ModuleId { get; set; }
    public Module Module { get; set; } = default!;

    public string Title { get; set; } = default!;
    public string? Instructions { get; set; }
    public DateTime? DueAt { get; set; }
    public decimal? MaxPoints { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
