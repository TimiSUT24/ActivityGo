using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Category.DTO
{
    public record CategoryUpdateDto(string Name, string? Description, bool? IsActive);
}
