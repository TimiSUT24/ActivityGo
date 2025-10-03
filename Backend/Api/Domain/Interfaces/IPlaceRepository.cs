using Domain.Models;

namespace Domain.Interfaces;

public interface IPlaceRepository : IGenericRepository<Place>
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
    Task<IEnumerable<Place>> GetActiveAsync(CancellationToken ct);
}

