using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IActivityRepository : IGenericRepository<SportActivity>
    {
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SportActivity>> GetActiveAsync(CancellationToken cancellationToken = default);
        IQueryable<SportActivity> Query();
    }
}
