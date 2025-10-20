using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Category.DTO
{
    public record CategoryReadDto(Guid Id, string Name, string? Description, bool IsActive);
}
