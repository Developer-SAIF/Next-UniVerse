using backend.Entities;

namespace backend.Contracts.Courses;

public sealed record InstructorCourseListItemDto(
    long Id,
    string Title,
    string? Description,
    string? Language,
    CourseLevel? Level,
    bool IsPublished,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
