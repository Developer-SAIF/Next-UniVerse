namespace backend.Entities;

public class Question
{
    public long Id { get; set; }

    public long QuizId { get; set; }
    public Quiz Quiz { get; set; } = default!;

    public QuestionType Type { get; set; }
    public string? Prompt { get; set; }
    public decimal Points { get; set; }
    public int? Position { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Choice> Choices { get; set; } = new List<Choice>();
    public ICollection<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
}
