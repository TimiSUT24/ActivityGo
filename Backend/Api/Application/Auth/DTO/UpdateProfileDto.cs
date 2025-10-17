using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.DTO
{
    public record UpdateProfileDto(
        string FirstName,
        string LastName,
        string? Email
    );
}
