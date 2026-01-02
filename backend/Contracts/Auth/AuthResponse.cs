namespace backend.Contracts.Auth;

public sealed record AuthResponse(
    string AccessToken,
    UserDto User
);
