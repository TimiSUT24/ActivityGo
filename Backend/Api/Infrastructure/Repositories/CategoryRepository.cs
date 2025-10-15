using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;
public sealed class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        => await _dbSet.AnyAsync(c => c.Name == name, ct);
}
