using System.Security.Claims;
using Application.Auth.DTO;         // RegisterDto, LoginDto, AuthResult, RefreshRequest (lägg till denna DTO)
using Application.Auth.Interface;  // ITokenService
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _users;
        private readonly SignInManager<User> _signIn;

        public AuthController(ITokenService tokenService, UserManager<User> users, SignInManager<User> signIn)
        {
            _tokenService = tokenService;
            _users = users;
            _signIn = signIn;
        }

        // === Helpers för refresh-cookie ===
        private void SetRefreshCookie(string refreshToken)
        {
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(14) // matcha Jwt:RefreshDays
            });
        }

        private void ClearRefreshCookie() => Response.Cookies.Delete("refreshToken");

        // === Register (skapar user + valfri default-roll) ===
        [HttpPost("register")]
        [AllowAnonymous] // Tillåter anonyma användare att komma åt denna endpoint
        public async Task<ActionResult<AuthResult>> Register([FromBody] RegisterDto dto, CancellationToken ct)
        {
            // Finns användaren redan?
            var existingUser = await _users.FindByEmailAsync(dto.Email);
            if (existingUser is not null)
                return Conflict(new { message = "User with this email already exists." });

            var user = new User
            {
                Email = dto.Email,
                Firstname = dto.Firstname,
                Lastname = dto.Lastname,
                UserName = dto.Email,
                EmailConfirmed = true // dev: slipp mailflöde
            };

            var create = await _users.CreateAsync(user, dto.Password);
            if (!create.Succeeded)
                return BadRequest(create.Errors);

            // Tilldela standardroll (seeda rollen 'User' vid uppstart)
            await _users.AddToRoleAsync(user, "User");

            // Skapa access + refresh, sätt cookie och returnera AuthResult (access token)
            var (access, refresh) = await _tokenService.IssueTokensAsync(user, HttpContext.Connection.RemoteIpAddress?.ToString());
            SetRefreshCookie(refresh);
            return Ok(new AuthResult(access));
        }

        // === Login (returnerar JWT-token + sätter refresh-cookie) ===
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResult>> Login([FromBody] LoginDto dto, CancellationToken ct)
        {
            var user = await _users.FindByEmailAsync(dto.Email);
            if (user is null)
                return Unauthorized(new { message = "Invalid email or password." });

            // Viktigt: använd cookiesless variant för JWT
            var result = await _signIn.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid email or password." });

            var (access, refresh) = await _tokenService.IssueTokensAsync(user, HttpContext.Connection.RemoteIpAddress?.ToString());
            SetRefreshCookie(refresh);
            return Ok(new AuthResult(access));
        }

        // === Refresh (roterar refresh-token och ger ny access-token) ===
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResult>> Refresh([FromBody] RefreshRequest? body)
        {
            // Hämta från cookie i första hand, annars från body
            var tokenFromCookie = Request.Cookies["refreshToken"];
            var presented = body?.RefreshToken ?? tokenFromCookie;
            if (string.IsNullOrWhiteSpace(presented))
                return BadRequest(new { message = "Missing refresh token." });

            try
            {
                var (access, newRefresh) = await _tokenService.RefreshAsync(presented, HttpContext.Connection.RemoteIpAddress?.ToString());
                SetRefreshCookie(newRefresh); // rotation
                return Ok(new AuthResult(access));
            }
            catch
            {
                ClearRefreshCookie();
                return Unauthorized(new { message = "Invalid refresh token." });
            }
        }

        // === Logout (revokerar alla refresh-tokens för användaren) ===
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            await _tokenService.RevokeAllAsync(userId, HttpContext.Connection.RemoteIpAddress?.ToString());
            ClearRefreshCookie();
            return NoContent();
        }

        // === Me (hämtar info om inloggad user) ===
        [HttpGet("me")]
        [Authorize]
        public ActionResult<object> Me()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value;
            var email = User.FindFirstValue(ClaimTypes.Email);
            var displayName = User.FindFirst("displayName")?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

            return Ok(new
            {
                id,
                email,
                name = displayName,
                roles
            });
        }
    }
}