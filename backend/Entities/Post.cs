namespace backend.Entities;

public class Post
{
    public long Id { get; set; }

    public long ThreadId { get; set; }
    public DiscussionThread Thread { get; set; } = default!;

    public long AuthorId { get; set; }
    public User Author { get; set; } = default!;

    public string? Body { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
