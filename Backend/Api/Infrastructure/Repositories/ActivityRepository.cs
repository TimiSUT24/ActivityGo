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

        public IQueryable<SportActivity> Query()
        {
            return _dbSet.AsQueryable();
        }
    }
}
