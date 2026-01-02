using backend.Entities;

namespace backend.Contracts.Auth;

public sealed record UserDto(
    long Id,
    string Name,
    string Email,
    UserRole Role,
    UserStatus Status
);
