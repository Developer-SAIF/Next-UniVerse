using backend.Entities;

namespace backend.Contracts.Courses;

public sealed record UpdateCourseRequest(
    string Title,
    string? Description,
    string? Language,
    CourseLevel? Level
);
