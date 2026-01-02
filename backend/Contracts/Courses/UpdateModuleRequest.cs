namespace backend.Contracts.Courses;

public sealed record UpdateModuleRequest(
    string Title,
    int? Position,
    string? Description
);
