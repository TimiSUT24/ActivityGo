using Application.Activity.DTO.Request;
using Application.Activity.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Activity.Interface
{
    public interface IActivityService
    {
        Task<ActivityResponse> CreateAsync(ActivityCreateRequest request, CancellationToken cancellationToken);
        Task<ActivityResponse> GetAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<ActivityResponse>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Guid id, ActivityUpdateRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
