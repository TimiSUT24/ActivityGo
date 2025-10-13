namespace Application.Auth.DTO;

public record AuthResult(
    string AccessToken,
    string? RefreshToken = null,
    string? UserId = null,
    string? Email = null
);