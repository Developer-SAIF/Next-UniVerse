using backend.Entities;

namespace backend.Contracts.Courses;

public sealed record CreateCourseRequest(
    string Title,
    string? Description,
    string? Language,
    CourseLevel? Level
);
