using Application.ActivityOccurrence.DTO.Request;
using Application.ActivityOccurrence.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityOccurrence.Interface
{
    public interface IActivityOccurrenceService
    {
        Task<ActivityOccurrenceDto> AddAsync(CreateActivityOccurenceDto dto, CancellationToken ct);
        Task<IEnumerable<ActivityOccurrenceDto>> GetAllAsync(CancellationToken ct);
        Task<ActivityOccurrenceDto?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<bool> UpdateAsync(UpdateActivityOccurenceDto dto, CancellationToken ct);
    }
}
