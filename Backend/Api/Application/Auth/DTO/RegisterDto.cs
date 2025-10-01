using System.ComponentModel.DataAnnotations;

namespace Application.Auth.DTO;

public record RegisterDto(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    string? Firstname,
    string? Lastname
);