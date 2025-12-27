namespace backend.Entities;

public class DiscussionThread
{
    public long Id { get; set; }

    public long CourseId { get; set; }
    public Course Course { get; set; } = default!;

    public long? ModuleId { get; set; }
    public Module? Module { get; set; }

    public string Title { get; set; } = default!;
    public bool IsLocked { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public long? CreatedBy { get; set; }

    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
