namespace backend.Contracts.Courses;

public sealed record CreateModuleRequest(
    string Title,
    int? Position,
    string? Description
);
