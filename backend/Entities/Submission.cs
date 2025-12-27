namespace backend.Entities;

public class Submission
{
    public long Id { get; set; }

    public long AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = default!;

    public long UserId { get; set; }
    public User User { get; set; } = default!;

    public DateTime SubmittedAt { get; set; }
    public string? StorageUrl { get; set; }
    public decimal? Grade { get; set; }
    public string? Feedback { get; set; }
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
