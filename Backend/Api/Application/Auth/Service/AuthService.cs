using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Auth.DTO;
using Application.Auth.Interface;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Auth.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _users;
        private readonly SignInManager<User> _signIn;
        private readonly ITokenService _tokens;
        private readonly IHttpContextAccessor? _http;

        public AuthService(
            UserManager<User> users,
            SignInManager<User> signIn,
            ITokenService tokens,
            IHttpContextAccessor? http = null)
        {
            _users = users;
            _signIn = signIn;
            _tokens = tokens;
            _http = http;
        }

        private string? GetIp() =>
            _http?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        public async Task<AuthResult> LoginAsync(LoginDto dto, CancellationToken ct)
        {
            var user = await _users.FindByEmailAsync(dto.Email);
            if (user is null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var check = await _signIn.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
            if (!check.Succeeded)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var (access, refresh) = await _tokens.IssueTokensAsync(user, GetIp());
            return new AuthResult(access)
            {
                RefreshToken = refresh,
                Email = user.Email,
                UserId = user.Id
            };
        }

        public async Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("Missing refresh token.", nameof(refreshToken));

            var (newAccess, newRefresh) = await _tokens.RefreshAsync(refreshToken, GetIp());
            return new AuthResult(
                AccessToken: newAccess,
                RefreshToken: newRefresh
            );
        }

        public async Task LogoutAsync(string userId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return;

            await _tokens.RevokeAllAsync(userId, GetIp());
        }

        // Valfritt – om du vill samla även register här
        public async Task<AuthResult> RegisterAsync(RegisterDto dto, CancellationToken ct)
        {
            var existing = await _users.FindByEmailAsync(dto.Email);
            if (existing is not null)
                throw new InvalidOperationException("User with this email already exists.");

            var user = new User
            {
                Email = dto.Email,
                Firstname = dto.Firstname,
                Lastname = dto.Lastname,
                UserName = dto.Email,
                EmailConfirmed = true // dev: skippa e-postflöde
            };

            var create = await _users.CreateAsync(user, dto.Password);
            if (!create.Succeeded)
                throw new InvalidOperationException(string.Join("; ", create.Errors));

            // Seedad standardroll "User" om du använder roller
            await _users.AddToRoleAsync(user, "User");

            var (access, refresh) = await _tokens.IssueTokensAsync(user, GetIp());
            return new AuthResult(
                AccessToken: access,
                RefreshToken: refresh,
                UserId: user.Id,
                Email: user.Email
            );
        }
    }
}
