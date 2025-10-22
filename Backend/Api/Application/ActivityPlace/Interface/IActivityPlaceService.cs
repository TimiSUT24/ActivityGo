using Application.ActivityPlace.DTO.Request;
using Application.ActivityPlace.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityPlace.Interface
{
    public interface IActivityPlaceService
    {
        Task<bool> AddAsync(CreateActivityPlaceDto dto, CancellationToken ct);
        Task<List<GetActivityPlaceDto>> GetPlaceForActivity(Guid id, CancellationToken ct);
        Task<bool> UpdateAsync(CreateActivityPlaceDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(CreateActivityPlaceDto dto, CancellationToken ct);
        Task<IEnumerable<GetAllActivityPlaceDto>> GetAllAsync(CancellationToken ct);
    }
}
