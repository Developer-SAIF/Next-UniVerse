using backend.Entities;

namespace backend.Contracts.Enrollments;

public sealed record EnrollmentDto(
    long Id,
    long CourseId,
    string CourseTitle,
    EnrollmentStatus Status,
    decimal Progress,
    DateOnly EnrolledOn
);
