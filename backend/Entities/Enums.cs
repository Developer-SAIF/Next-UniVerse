namespace backend.Entities;

public enum UserRole
{
    Student,
    Instructor,
    Admin
}

public enum UserStatus
{
    Active,
    Suspended,
    Pending
}

public enum CourseLevel
{
    Beginner,
    Intermediate,
    Advanced
}

public enum EnrollmentStatus
{
    Active,
    Completed,
    Cancelled
}

public enum MaterialKind
{
    Video,
    Pdf,
    Slide,
    Html,
    Audio,
    Other
}

public enum QuestionType
{
    Mcq,
    MultiSelect,
    ShortText,
    LongText,
    Numeric
}

public enum QuizAttemptStatus
{
    InProgress,
    Submitted,
    Graded,
    Abandoned
}

public enum SubmissionStatus
{
    Submitted,
    Graded,
    Late,
    Resubmitted
}

public enum CertificateStatus
{
    Issued,
    Revoked
}

public enum NotificationType
{
    System,
    Course,
    Assignment,
    Message
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}

public enum RelationshipType
{
    Follower,
    Following,
    Blocked,
    Colleague
}
