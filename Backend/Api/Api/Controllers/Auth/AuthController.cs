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
        private readonly IAuthService _authService;

        public AuthController(ITokenService tokenService, UserManager<User> users, SignInManager<User> signIn, IAuthService authService)
        {
            _tokenService = tokenService;
            _users = users;
            _signIn = signIn;
            _authService = authService;
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
            var register = await _authService.RegisterAsync(dto, ct);
            return CreatedAtAction(nameof(Register), new { id = register.Email }, register);
        }

        // === Login (returnerar JWT-token + sätter refresh-cookie) ===
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResult>> Login([FromBody] LoginDto dto, CancellationToken ct)
        {
            var login = await _authService.LoginAsync(dto, ct);
            return Ok(login);
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