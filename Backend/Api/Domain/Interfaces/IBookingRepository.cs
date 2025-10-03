using Domain.Enums;
using Domain.Models;
using Domain.Reporting;

namespace Domain.Interfaces;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<int> CountActiveForOccurrenceAsync(Guid activityOccurrenceId, CancellationToken ct);
    Task<bool> ExistsOverlapForUserAsync(string userId, DateTime startUtc, DateTime endUtc, CancellationToken ct);
    Task<IEnumerable<Booking>> GetByUserAsync(string userId, CancellationToken ct);
    Task<IEnumerable<Booking>> GetByUserAndStatusAsync(string userId, BookingStatus status, CancellationToken ct);
    Task<Booking?> GetByIdForUserAsync(Guid id, string userId, CancellationToken ct);

    Task<ActivityOccurrence?> GetOccurrenceWithRefsAsync(Guid occurrenceId, CancellationToken ct);
    
    Task<int> CountInRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct);
    Task<int> CountByStatusInRangeAsync(BookingStatus status, DateTime fromUtc, DateTime toUtc, CancellationToken ct);

    /// <summary>Summa intäkter för Completed bokningar i intervallet.
    /// Pris = Occurrence.PriceOverride ?? Activity.Price</summary>
    Task<decimal> SumRevenueCompletedInRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct);
    
    Task<IEnumerable<CountBucket>>  GetBookingsPerDayAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct);
    Task<IEnumerable<RevenueBucket>> GetRevenuePerDayAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct);

    Task<IEnumerable<TopItem>> GetTopActivitiesAsync(DateTime fromUtc, DateTime toUtc, int take, CancellationToken ct);
    Task<IEnumerable<TopItem>> GetTopPlacesAsync(DateTime fromUtc, DateTime toUtc, int take, CancellationToken ct);
    Task<IEnumerable<TopItem>> GetByCategoryAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct);
}
