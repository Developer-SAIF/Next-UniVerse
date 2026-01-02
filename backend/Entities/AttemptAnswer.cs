namespace backend.Entities;

public class AttemptAnswer
{
    public long Id { get; set; }

    public long AttemptId { get; set; }
    public QuizAttempt Attempt { get; set; } = default!;

    public long QuestionId { get; set; }
    public Question Question { get; set; } = default!;

    public string? AnswerText { get; set; }

    public long? SelectedChoiceId { get; set; }
    public Choice? SelectedChoice { get; set; }

    public bool? IsCorrect { get; set; }
    public decimal? AwardedPoints { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
