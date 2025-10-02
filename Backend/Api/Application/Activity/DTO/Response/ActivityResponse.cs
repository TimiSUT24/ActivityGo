using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Activity.DTO.Response
{
    // DTO for returning activity details
    public sealed record ActivityResponse
    (
        Guid Id,
        string Name,
        string? Description,
        Guid? CategoryId,
        string? CategoryName,
        int DefaultDurationMinutes,
        decimal Price,
        string? ImageUrl,
        int Environment,
        bool IsActive,
        DateTime CreatedAtUtc
    );
}
