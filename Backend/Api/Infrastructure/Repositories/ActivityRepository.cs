using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public sealed class ActivityRepository : GenericRepository<SportActivity>, IActivityRepository
    {
        public ActivityRepository(AppDbContext context) : base(context)
        {
        }

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return _dbSet.AnyAsync(a => a.Name == name, cancellationToken);
        }

        public async Task<IReadOnlyList<SportActivity>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(a => a.IsActive).ToListAsync(cancellationToken);
        }

        public Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
            => _dbSet
                .AsNoTracking()
                .CountAsync(a => a.IsActive, cancellationToken);

        public IQueryable<SportActivity> Query()
            => _dbSet.AsNoTracking();
    }
}
