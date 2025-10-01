using System.ComponentModel.DataAnnotations;

namespace Application.Auth.DTO;

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password
);