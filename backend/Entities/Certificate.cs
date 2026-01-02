namespace backend.Entities;

public class Certificate
{
    public long Id { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = default!;

    public long CourseId { get; set; }
    public Course Course { get; set; } = default!;

    public string? Serial { get; set; }
    public DateTime IssuedAt { get; set; }
    public CertificateStatus Status { get; set; } = CertificateStatus.Issued;

    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
