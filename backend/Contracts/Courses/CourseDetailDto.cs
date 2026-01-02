using backend.Entities;

namespace backend.Contracts.Courses;

public sealed record CourseDetailDto(
    long Id,
    string Title,
    string? Description,
    string? Language,
    CourseLevel? Level,
    bool IsPublished,
    long InstructorId,
    string InstructorName,
    IReadOnlyList<ModuleDto> Modules
);
