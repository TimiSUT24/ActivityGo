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
    public class ActivityPlaceRepository : GenericRepository<ActivityPlace>, IActivityPlaceRepository
    {
        private readonly AppDbContext _db;

        public ActivityPlaceRepository(AppDbContext db) : base(db) => _db = db;

        public async Task<List<ActivityPlace>> GetPlaceForActivityAsync(Guid id, CancellationToken ct)
        {
            var places = await _db.ActivityPlaces
                .Where(ap => ap.SportActivityId == id)
                .Include(ap => ap.Place)
                .ToListAsync(ct);

            return places;
        }

        public async Task<ActivityPlace?> FirstOrDefaultAsync(Func<ActivityPlace, bool> predicate, CancellationToken ct)
        {
            return await Task.FromResult(_db.ActivityPlaces.AsNoTracking().FirstOrDefault(predicate));
        }

        public override async Task<IEnumerable<ActivityPlace>> GetAllAsync(CancellationToken ct)
        {
            var entities = await _db.ActivityPlaces
                .Include(ap => ap.Place)
                .Include(ap => ap.SportActivity)
                .ToListAsync(ct);

            return entities;
        }

    }
}
