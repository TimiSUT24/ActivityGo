using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IActivityPlaceRepository : IGenericRepository<ActivityPlace>
    {
        Task<List<ActivityPlace>> GetPlaceForActivityAsync(Guid id, CancellationToken ct);
        Task<ActivityPlace?> FirstOrDefaultAsync(Func<ActivityPlace, bool> predicate, CancellationToken ct);
        Task<IEnumerable<ActivityPlace>> GetAllAsync(CancellationToken ct);
    }
}
