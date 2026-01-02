namespace backend.Contracts.Courses;

public sealed record ModuleDto(
    long Id,
    string Title,
    int? Position,
    string? Description,
    IReadOnlyList<MaterialDto> Materials
);
