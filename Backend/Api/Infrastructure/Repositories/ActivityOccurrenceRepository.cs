using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public sealed class ActivityOccurrenceRepository : GenericRepository<ActivityOccurrence>, IActivityOccurrenceRepository
{
    private readonly AppDbContext _db;

    public ActivityOccurrenceRepository(AppDbContext db) : base(db) => _db = db;

    public override async Task<ActivityOccurrence?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.ActivityOccurrences
            .AsNoTracking()
            .Include(o => o.Place)
            .Include(o => o.Activity).ThenInclude(a => a.Category)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public override async Task<IEnumerable<ActivityOccurrence>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.ActivityOccurrences
            .AsNoTracking()
            .Include(o => o.Place)
            .Include(o => o.Activity)
                .ThenInclude(a => a.Category)
            .ToListAsync(ct);
    }

    // Method to improve query for alternative filters
    public async Task<IReadOnlyList<ActivityOccurrence>> GetBetweenDatesFilteredAsync(
        DateTime fromDate, DateTime toDate,
        Guid? categoryId, Guid? activityId, Guid? placeId,
        EnvironmentType? environment, bool? onlyAvailable,
        CancellationToken ct)
    {
        var q = _db.ActivityOccurrences
            .AsNoTracking()
            .Include(o => o.Activity)
            .ThenInclude(a => a.Category)
            .Include(o => o.Place)
            .Include(o => o.Bookings)
            .Where(o => o.StartUtc >= fromDate && o.StartUtc < toDate)
            .Where(o => o.Place.IsActive && o.Activity.IsActive);

        if (categoryId.HasValue)
            q = q.Where(o => o.Activity.CategoryId == categoryId.Value);

        if (activityId.HasValue)
            q = q.Where(o => o.ActivityId == activityId.Value);

        if (placeId.HasValue)
            q = q.Where(o => o.PlaceId == placeId.Value);

        if (environment.HasValue)
            q = q.Where(o => o.Place.Environment == environment.Value);

        if (onlyAvailable == true)
        {
            q = q.Where(o =>
                _db.Bookings.Count(b => b.ActivityOccurrenceId == o.Id && b.Status == BookingStatus.Booked)
                < (o.CapacityOverride.HasValue ? o.CapacityOverride.Value : o.Place.Capacity));
        }


        return await q.OrderBy(o => o.StartUtc).ToListAsync(ct);

    }


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