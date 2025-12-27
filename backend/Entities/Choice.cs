namespace backend.Entities;

public class Choice
{
    public long Id { get; set; }

    public long QuestionId { get; set; }
    public Question Question { get; set; } = default!;

    public string? Text { get; set; }
    public bool IsCorrect { get; set; }
    public int? Position { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
