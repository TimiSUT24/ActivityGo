using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Activity.DTO.Request
{
    // DTO for updating an existing activity
    public sealed record ActivityUpdateRequest
    (
        string Name,
        string? Description,
        Guid? CategoryId,
        int DefaultDurationMinutes,
        decimal Price,
        string? ImageUrl,
        int Environment,
        bool IsActive
    );

}
