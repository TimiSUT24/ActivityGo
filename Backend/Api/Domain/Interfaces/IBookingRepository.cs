using Domain.Enums;
using Domain.Models;

namespace Domain.Interfaces;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<int> CountActiveForOccurrenceAsync(Guid activityOccurrenceId, CancellationToken ct);
    Task<bool> ExistsOverlapForUserAsync(string userId, DateTime startUtc, DateTime endUtc, CancellationToken ct);
    Task<IEnumerable<Booking>> GetByUserAsync(string userId, CancellationToken ct);
    Task<IEnumerable<Booking>> GetByUserAndStatusAsync(string userId, BookingStatus status, CancellationToken ct);
    Task<Booking?> GetByIdForUserAsync(Guid id, string userId, CancellationToken ct);

    Task<ActivityOccurrence?> GetOccurrenceWithRefsAsync(Guid occurrenceId, CancellationToken ct);
}
