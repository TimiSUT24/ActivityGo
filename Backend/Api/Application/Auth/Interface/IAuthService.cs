using Application.Auth.DTO;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Auth.Interface
{
    public interface IAuthService
    {
        Task<(AuthResult? Result, string? RefreshToken)> LoginAsync(LoginDto dto, string? ipAddress, CancellationToken cancellationToken);
        Task<(AuthResult? Result, string? RefreshToken)> RefreshAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken);
        Task LogoutAsync(string userId, string? ipAddress, CancellationToken cancellationToken);
    }
}
