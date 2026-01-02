namespace backend.Entities;

public class Module
{
    public long Id { get; set; }

    public long CourseId { get; set; }
    public Course Course { get; set; } = default!;

    public string Title { get; set; } = default!;
    public int? Position { get; set; }
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Material> Materials { get; set; } = new List<Material>();
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<DiscussionThread> DiscussionThreads { get; set; } = new List<DiscussionThread>();
}
