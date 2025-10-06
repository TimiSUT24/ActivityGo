using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ActivityOccurrenceRepository : GenericRepository<ActivityOccurrence>, IActivityOccurrenceRepository
{
    private readonly AppDbContext _db;

    public ActivityOccurrenceRepository(AppDbContext db) : base(db) => _db = db;

    public async Task<IReadOnlyList<OccurrenceUtilItem>> GetUtilizationItemsAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct)
    {
        return await _db.ActivityOccurrences.AsNoTracking()
            .Where(o => o.StartUtc >= fromUtc && o.StartUtc < toUtc)
            .Select(o => new OccurrenceUtilItem
            {
                OccurrenceId = o.Id,
                Capacity = o.CapacityOverride ?? o.Place.Capacity,
                NonCancelledBookings = o.Bookings.Count(b => b.Status != BookingStatus.Cancelled)
            })
            .ToListAsync(ct);
    }
}