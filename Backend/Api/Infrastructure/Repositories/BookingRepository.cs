using Application.Statistics.DTO;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Domain.Reporting;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    private readonly AppDbContext _db;
    public BookingRepository(AppDbContext db) : base(db) => _db = db;

    public async Task<int> CountActiveForOccurrenceAsync(Guid activityOccurrenceId, CancellationToken ct) =>
        await _db.Bookings.CountAsync(b =>
            b.ActivityOccurrenceId == activityOccurrenceId &&
            b.Status == BookingStatus.Booked, ct);

    public async Task<bool> ExistsOverlapForUserAsync(string userId, DateTime startUtc, DateTime endUtc, CancellationToken ct) =>
        await _db.Bookings
            .AsNoTracking()
            .AnyAsync(b =>
                b.UserId == userId &&
                b.Status == BookingStatus.Booked &&
                b.ActivityOccurrence.StartUtc < endUtc &&
                b.ActivityOccurrence.EndUtc > startUtc, ct);

    public async Task<IEnumerable<Booking>> GetByUserAsync(string userId, CancellationToken ct) =>
        await _db.Bookings
            .AsNoTracking()
            .AsSplitQuery()
            .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Place)
            .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Activity)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookedAtUtc)
            .ToListAsync(ct);

    public async Task<IEnumerable<Booking>> GetByUserAndStatusAsync(string userId, BookingStatus status, CancellationToken ct) =>
        await _db.Bookings
            .AsNoTracking()
            .AsSplitQuery()
            .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Place)
            .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Activity)
            .Where(b => b.UserId == userId && b.Status == status)
            .OrderBy(b => b.ActivityOccurrence.StartUtc)
            .ToListAsync(ct);

    public async Task<Booking?> GetByIdForUserAsync(Guid id, string userId, CancellationToken ct) =>
        await _db.Bookings
            .AsNoTracking()
            .AsSplitQuery()
            .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Place)
            .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Activity)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId, ct);

    public Task<ActivityOccurrence?> GetOccurrenceWithRefsAsync(Guid occurrenceId, CancellationToken ct) =>
        _db.ActivityOccurrences
            .Include(o => o.Place)
            .Include(o => o.Activity)
            .FirstOrDefaultAsync(o => o.Id == occurrenceId, ct);
    
    // Statistikmetoder
public Task<int> CountInRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct)
    => _db.Bookings.AsNoTracking()
        .Include(b => b.ActivityOccurrence)
        .Where(b => b.ActivityOccurrence.StartUtc >= fromUtc && b.ActivityOccurrence.StartUtc < toUtc)
        .CountAsync(ct);

public Task<int> CountByStatusInRangeAsync(BookingStatus status, DateTime fromUtc, DateTime toUtc, CancellationToken ct)
    => _db.Bookings.AsNoTracking()
        .Include(b => b.ActivityOccurrence)
        .Where(b => b.ActivityOccurrence.StartUtc >= fromUtc && b.ActivityOccurrence.StartUtc < toUtc)
        .Where(b => b.Status == status)
        .CountAsync(ct);

public Task<decimal> SumRevenueCompletedInRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct)
    => _db.Bookings.AsNoTracking()
        .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Activity)
        .Where(b => b.ActivityOccurrence.StartUtc >= fromUtc && b.ActivityOccurrence.StartUtc < toUtc)
        .Where(b => b.Status == BookingStatus.Completed)
        .Select(b => b.ActivityOccurrence.PriceOverride ?? b.ActivityOccurrence.Activity.Price)
        .SumAsync(ct);

public async Task<IEnumerable<CountBucket>> GetBookingsPerDayAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct)
    => await _db.Bookings.AsNoTracking()
        .Include(b => b.ActivityOccurrence)
        .Where(b => b.ActivityOccurrence.StartUtc >= fromUtc && b.ActivityOccurrence.StartUtc < toUtc)
        .GroupBy(b => b.ActivityOccurrence.StartUtc.Date)
        .Select(g => new CountBucket(g.Key, g.Count()))
        .OrderBy(x => x.Bucket)
        .ToListAsync(ct);

public async Task<IEnumerable<RevenueBucket>> GetRevenuePerDayAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct)
    => await _db.Bookings.AsNoTracking()
        .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Activity)
        .Where(b => b.ActivityOccurrence.StartUtc >= fromUtc && b.ActivityOccurrence.StartUtc < toUtc)
        .Where(b => b.Status == BookingStatus.Completed)
        .GroupBy(b => b.ActivityOccurrence.StartUtc.Date)
        .Select(g => new RevenueBucket(
            g.Key,
            g.Sum(b => b.ActivityOccurrence.PriceOverride ?? b.ActivityOccurrence.Activity.Price)
        ))
        .OrderBy(x => x.Bucket)
        .ToListAsync(ct);

public async Task<IEnumerable<TopItem>> GetTopActivitiesAsync(DateTime fromUtc, DateTime toUtc, int take, CancellationToken ct)
    => await _db.Bookings.AsNoTracking()
        .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Activity)
        .Where(b => b.ActivityOccurrence.StartUtc >= fromUtc && b.ActivityOccurrence.StartUtc < toUtc)
        .GroupBy(b => new { b.ActivityOccurrence.ActivityId, b.ActivityOccurrence.Activity.Name })
        .Select(g => new TopItem(g.Key.ActivityId, g.Key.Name, g.Count()))
        .OrderByDescending(x => x.Count)
        .Take(take)
        .ToListAsync(ct);

public async Task<IEnumerable<TopItem>> GetTopPlacesAsync(DateTime fromUtc, DateTime toUtc, int take, CancellationToken ct)
    => await _db.Bookings.AsNoTracking()
        .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Place)
        .Where(b => b.ActivityOccurrence.StartUtc >= fromUtc && b.ActivityOccurrence.StartUtc < toUtc)
        .GroupBy(b => new { b.ActivityOccurrence.PlaceId, b.ActivityOccurrence.Place.Name })
        .Select(g => new TopItem(g.Key.PlaceId, g.Key.Name, g.Count()))
        .OrderByDescending(x => x.Count)
        .Take(take)
        .ToListAsync(ct);

public async Task<IEnumerable<TopItem>> GetByCategoryAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct)
    => await _db.Bookings.AsNoTracking()
        .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Activity).ThenInclude(a => a.Category)
        .Where(b => b.ActivityOccurrence.StartUtc >= fromUtc && b.ActivityOccurrence.StartUtc < toUtc)
        .GroupBy(b => new
        {
            b.ActivityOccurrence.Activity.CategoryId,
            CategoryName = b.ActivityOccurrence.Activity.Category != null
                ? b.ActivityOccurrence.Activity.Category.Name
                : "(Okänd kategori)"
        })
        .Select(g => new TopItem(g.Key.CategoryId ?? Guid.Empty, g.Key.CategoryName, g.Count()))
        .OrderByDescending(x => x.Count)
        .ToListAsync(ct);
}
