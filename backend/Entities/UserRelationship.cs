namespace backend.Entities;

public class UserRelationship
{
    public long Id { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = default!;

    public long RelatedUserId { get; set; }
    public User RelatedUser { get; set; } = default!;

    public RelationshipType RelationshipType { get; set; }

    public DateTime CreatedAt { get; set; }
}
