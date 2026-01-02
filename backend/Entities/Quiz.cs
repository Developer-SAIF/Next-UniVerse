namespace backend.Entities;

public class Quiz
{
    public long Id { get; set; }

    public long ModuleId { get; set; }
    public Module Module { get; set; } = default!;

    public string Title { get; set; } = default!;
    public int? TimeLimitSeconds { get; set; }
    public int AttemptsAllowed { get; set; }
    public bool ShuffleQuestions { get; set; }
    public decimal? PassScore { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}
