using backend.Entities;

namespace backend.Contracts.Courses;

public sealed record MaterialDto(
    long Id,
    MaterialKind Kind,
    string Title,
    string? Description,
    string? StorageUrl,
    string? MimeType,
    long? SizeBytes,
    bool RequiresAuth,
    int? Position
);
