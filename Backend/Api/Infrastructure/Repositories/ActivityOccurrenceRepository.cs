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

    // För att minimera fel som kan ske på olika sökningar med vissa tecken, då SQL har vissa tecken med särskild betydelse
    private static string EscapeLike(string s) => s
    .Replace("[", "[[]")
    .Replace("]", "[]]")
    .Replace("%", "[%]")
    .Replace("_", "[_]");

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
        DateTime fromDate, 
        DateTime toDate,
        Guid? categoryId, 
        Guid? activityId, 
        Guid? placeId,
        EnvironmentType? environment,
        string? freeTextSearch,
        CancellationToken ct)
    {
        var q = _db.ActivityOccurrences
            .AsNoTracking()
            .AsSplitQuery()
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

        if (!string.IsNullOrWhiteSpace(freeTextSearch))
        {
            var s = "%" + EscapeLike(freeTextSearch.Trim()) + "%";
            q = q.Where(o =>
            EF.Functions.Like(o.Activity.Name, s) ||
            EF.Functions.Like(o.Place.Name, s) ||
            EF.Functions.Like(o.Activity.Description, s) ||
            (o.Activity.Category != null && EF.Functions.Like(o.Activity.Category.Name, s)));
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