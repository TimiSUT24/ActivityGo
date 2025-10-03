using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
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
            .Include(b => b.ActivityOccurrence)
            .AnyAsync(b =>
                b.UserId == userId &&
                b.Status == BookingStatus.Booked &&
                b.ActivityOccurrence.StartUtc < endUtc &&
                b.ActivityOccurrence.EndUtc > startUtc, ct);

    public async Task<IEnumerable<Booking>> GetByUserAsync(string userId, CancellationToken ct) =>
        await _db.Bookings
            .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Place)
            .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Activity)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookedAtUtc)
            .ToListAsync(ct);

    public async Task<IEnumerable<Booking>> GetByUserAndStatusAsync(string userId, BookingStatus status, CancellationToken ct) =>
        await _db.Bookings
            .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Place)
            .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Activity)
            .Where(b => b.UserId == userId && b.Status == status)
            .OrderBy(b => b.ActivityOccurrence.StartUtc)
            .ToListAsync(ct);

    public async Task<Booking?> GetByIdForUserAsync(Guid id, string userId, CancellationToken ct) =>
        await _db.Bookings
            .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Place)
            .Include(b => b.ActivityOccurrence).ThenInclude(o => o.Activity)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId, ct);

    public Task<ActivityOccurrence?> GetOccurrenceWithRefsAsync(Guid occurrenceId, CancellationToken ct) =>
        _db.ActivityOccurrences
            .Include(o => o.Place)
            .Include(o => o.Activity)
            .FirstOrDefaultAsync(o => o.Id == occurrenceId, ct);
}
