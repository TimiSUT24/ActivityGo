using Domain.Models;

namespace Domain.Interfaces;
public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
}
