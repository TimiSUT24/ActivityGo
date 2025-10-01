namespace Application.Auth.Interface;

public interface ITokenService
{
    /// Skapa ett nytt access + refresh-token för given user (vid login/registrering).
    Task<(string accessToken, string refreshToken)> IssueTokensAsync(Domain.Models.User user, string? ip = null);

    /// Byt (rotera) refresh-token och ge tillbaka nytt access + refresh-token.
    Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken, string? ip = null);

    /// Revokera alla aktiva refresh-tokens för en användare (logout från alla enheter).
    Task RevokeAllAsync(string userId, string? ip = null);
}