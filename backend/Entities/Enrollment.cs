namespace backend.Entities;

public class Enrollment
{
    public long Id { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = default!;

    public long CourseId { get; set; }
    public Course Course { get; set; } = default!;

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public DateOnly EnrolledOn { get; set; }
    public DateOnly? CompletedOn { get; set; }
    public decimal Progress { get; set; } // $0.00 \le \text{progress} \le 100.00$

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}