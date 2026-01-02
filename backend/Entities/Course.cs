namespace backend.Entities;

public class Course
{
    public long Id { get; set; }

    public long InstructorId { get; set; }
    public User Instructor { get; set; } = default!;

    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? Language { get; set; }
    public CourseLevel? Level { get; set; }
    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigations
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public ICollection<Module> Modules { get; set; } = new List<Module>();
    public ICollection<DiscussionThread> DiscussionThreads { get; set; } = new List<DiscussionThread>();
    public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<CourseReview> Reviews { get; set; } = new List<CourseReview>();
}