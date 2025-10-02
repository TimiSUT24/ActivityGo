using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Activity.DTO.Request
{
    // DTO for creating a new activity
    public sealed record ActivityCreateRequest
    (
        string Name,
        string? Description,
        Guid? CategoryId,
        int DefaultDurationMinutes,
        decimal Price,
        string? ImageUrl,
        int Environment // EnvironmentType enum
        );
    
}
