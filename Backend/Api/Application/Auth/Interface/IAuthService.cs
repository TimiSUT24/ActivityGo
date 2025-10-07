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
        Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken ct);
        Task LogoutAsync(string userId, CancellationToken ct);

    }
}
