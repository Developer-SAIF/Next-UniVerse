namespace backend.Entities;

public class Payment
{
    public long Id { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = default!;

    public long? CourseId { get; set; }
    public Course? Course { get; set; }

    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime? PaymentDate { get; set; }
    public string? Provider { get; set; }
    public string? TransactionId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
