using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Auth.Interface;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Auth;
public sealed class TokenService : ITokenService
{
    private readonly UserManager<User> _users;
    private readonly IConfiguration _cfg;
    private readonly AppDbContext _db;

    public TokenService(UserManager<User> users, IConfiguration cfg, AppDbContext db)
    {
        _users = users;
        _cfg = cfg;
        _db = db;
    }

    // ===== Public API =====

    public async Task<(string accessToken, string refreshToken)> IssueTokensAsync(User user, string? ip = null)
    {
        var access = await CreateAccessTokenAsync(user);

        var refreshPlain = GenerateSecureToken();
        var refreshHash  = Sha256(refreshPlain);
        var days = int.Parse(_cfg["Jwt:RefreshDays"] ?? "14");

        var rt = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(days),
            CreatedByIp = ip
        };

        _db.RefreshTokens.Add(rt);
        await _db.SaveChangesAsync();

        return (access, refreshPlain); // skicka klartexten, hashen finns i DB
    }

    public async Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken, string? ip = null)
    {
        var now = DateTime.UtcNow;
        var hash = Sha256(refreshToken);

        var existing = await _db.RefreshTokens
            .AsTracking()
            .FirstOrDefaultAsync(x => x.TokenHash == hash);

        if (existing is null || existing.RevokedAtUtc is not null || existing.ExpiresAtUtc < now)
            throw new SecurityTokenException("Invalid refresh token");

        var user = await _users.FindByIdAsync(existing.UserId);
        if (user is null) throw new SecurityTokenException("User not found");

        // Rotera: revoka gamla och skapa ny
        existing.RevokedAtUtc = now;
        existing.RevokedByIp = ip;

        var newPlain = GenerateSecureToken();
        var newHash  = Sha256(newPlain);
        existing.ReplacedByTokenHash = newHash;

        var days = int.Parse(_cfg["Jwt:RefreshDays"] ?? "14");
        var next = new RefreshToken
        {
            UserId = existing.UserId,
            TokenHash = newHash,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(days),
            CreatedByIp = ip
        };
        _db.RefreshTokens.Add(next);

        await _db.SaveChangesAsync();

        var access = await CreateAccessTokenAsync(user);
        return (access, newPlain);
    }

    public async Task RevokeAllAsync(string userId, string? ip = null)
    {
        var now = DateTime.UtcNow;
        var tokens = await _db.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null && x.ExpiresAtUtc > now)
            .ToListAsync();

        foreach (var t in tokens)
        {
            t.RevokedAtUtc = now;
            t.RevokedByIp = ip;
        }

        await _db.SaveChangesAsync();
    }

    // ===== Interna helpers =====

    private async Task<string> CreateAccessTokenAsync(User user)
    {
        var roles = await _users.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? user.Email ?? user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new("displayName", $"{user.Firstname} {user.Lastname}".Trim())
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:ExpiresMinutes"] ?? "60")),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateSecureToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        // URL-safe Base64
        return Convert.ToBase64String(bytes)
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static string Sha256(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}