namespace backend.Entities;

public class QuizAttempt
{
    public long Id { get; set; }

    public long QuizId { get; set; }
    public Quiz Quiz { get; set; } = default!;

    public long UserId { get; set; }
    public User User { get; set; } = default!;

    public DateTime StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public decimal? Score { get; set; }
    public QuizAttemptStatus Status { get; set; } = QuizAttemptStatus.InProgress;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<AttemptAnswer> Answers { get; set; } = new List<AttemptAnswer>();
}
