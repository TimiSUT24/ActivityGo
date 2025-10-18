using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Auth.DTO;

namespace Application.Auth.Interface
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(LoginDto dto, CancellationToken cancellationToken);
        Task LogoutAsync(string userId, CancellationToken ct);
        Task<AuthResult> RegisterAsync(RegisterDto dto, CancellationToken ct);

        Task<AuthResult> UpdateProfileAsync(string userId, UpdateProfileDto dto, string? ip, CancellationToken ct);
        Task<AuthResult> ChangePasswordAsync(string userId, ChangePasswordDto dto, string? ip, CancellationToken ct);
    }
}
