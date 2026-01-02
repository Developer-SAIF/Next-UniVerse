using backend.Entities;

namespace backend.Contracts.Courses;

public sealed record UpdateMaterialRequest(
    MaterialKind Kind,
    string Title,
    string? Description,
    string? StorageUrl,
    string? MimeType,
    long? SizeBytes,
    bool RequiresAuth,
    int? Position
);
