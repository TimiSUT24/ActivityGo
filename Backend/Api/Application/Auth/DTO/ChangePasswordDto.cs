using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.DTO
{
    public record ChangePasswordDto(
        string CurrentPassword,
        string NewPassword
    );
}
