namespace backend.Entities;

public class CourseReview
{
    public long Id { get; set; }

    public long CourseId { get; set; }
    public Course Course { get; set; } = default!;

    public long UserId { get; set; }
    public User User { get; set; } = default!;

    public int? Rating { get; set; }
    public string? ReviewText { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
