namespace backend.Entities;

public class User
{
    public long Id { get; set; }

    public UserRole Role { get; set; } = UserRole.Student;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public UserStatus Status { get; set; } = UserStatus.Pending;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigations
    public ICollection<Course> CoursesTaught { get; set; } = new List<Course>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<AnalyticsEvent> AnalyticsEvents { get; set; } = new List<AnalyticsEvent>();
    public ICollection<UserRelationship> Relationships { get; set; } = new List<UserRelationship>();
    public ICollection<UserRelationship> RelatedRelationships { get; set; } = new List<UserRelationship>();
    public ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();
}