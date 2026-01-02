using backend.Entities;

namespace backend.Contracts.Courses;

public sealed record CourseListItemDto(
    long Id,
    string Title,
    string? Description,
    string? Language,
    CourseLevel? Level,
    bool IsPublished,
    long InstructorId,
    string InstructorName
);
