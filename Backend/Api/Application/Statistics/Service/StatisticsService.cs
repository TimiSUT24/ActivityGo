// Application/Statistics/Service/StatisticsService.cs
using Application.Statistics.DTO;
using Application.Statistics.Interface;
using Domain.Enums;
using Domain.Interfaces;         // IUnitOfWork
using System.Linq;              // Select, Average
using static System.Math;

namespace Application.Statistics.Service
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IUnitOfWork _uow;
        public StatisticsService(IUnitOfWork uow) => _uow = uow;

        private static (DateTime fromUtc, DateTime toUtc) Range(DateTime? from, DateTime? to)
        {
            var toUtc = (to ?? DateTime.UtcNow).ToUniversalTime();
            var fromUtc = (from ?? toUtc.AddDays(-30)).ToUniversalTime();
            return (fromUtc, toUtc);
        }

        public async Task<SummaryDto> GetSummaryAsync(DateTime? from, DateTime? to, CancellationToken ct)
        {
            var (fromUtc, toUtc) = Range(from, to);

            // Bas-KPI:er
            var totalUsers       = await _uow.Users.CountAsync(ct);
            var activeActivities = await _uow.Activities.CountActiveAsync(ct);
            var activePlaces     = await _uow.Places.CountActiveAsync(ct);

            // Bokningar i intervallet
            var total     = await _uow.Bookings.CountInRangeAsync(fromUtc, toUtc, ct);
            var booked    = await _uow.Bookings.CountByStatusInRangeAsync(BookingStatus.Booked,    fromUtc, toUtc, ct);
            var cancelled = await _uow.Bookings.CountByStatusInRangeAsync(BookingStatus.Cancelled, fromUtc, toUtc, ct);
            var completed = await _uow.Bookings.CountByStatusInRangeAsync(BookingStatus.Completed, fromUtc, toUtc, ct);

            // Intäkt (Completed) – pris = PriceOverride ?? Activity.Price
            var revenue = await _uow.Bookings.SumRevenueCompletedInRangeAsync(fromUtc, toUtc, ct);

            // Beläggning (utilization): snitt av (icke-cancelled / capacity) per occurrence
            var occUtilItems = await _uow.Occurrences.GetUtilizationItemsAsync(fromUtc, toUtc, ct);
            double avgUtil = 0;
            if (occUtilItems.Count > 0)
            {
                var ratios = occUtilItems
                    .Where(x => x.Capacity > 0)
                    .Select(x => Min(1.0, (double)x.NonCancelledBookings / x.Capacity))
                    .ToList();
                avgUtil = ratios.Count > 0 ? ratios.Average() * 100.0 : 0.0;
            }

            return new SummaryDto
            {
                FromUtc = fromUtc,
                ToUtc = toUtc,
                TotalUsers = totalUsers,
                ActiveActivities = activeActivities,
                ActivePlaces = activePlaces,
                TotalBookings = total,
                Booked = booked,
                Cancelled = cancelled,
                Completed = completed,
                EstimatedRevenue = revenue,
                AvgUtilizationPercent = Math.Round(avgUtil, 2),
                CancellationRatePercent = total > 0 ? Math.Round(cancelled * 100.0 / total, 2) : 0,
                CompletionRatePercent = total > 0 ? Math.Round(completed * 100.0 / total, 2) : 0
            };
        }

        public async Task<IEnumerable<BookingsPerBucketDto>> GetBookingsPerDayAsync(DateTime? from, DateTime? to, CancellationToken ct)
        {
            var (fromUtc, toUtc) = Range(from, to);
            var buckets = await _uow.Bookings.GetBookingsPerDayAsync(fromUtc, toUtc, ct);
            return buckets.Select(b => new BookingsPerBucketDto { Bucket = b.Bucket, Count = b.Count });
        }

        public async Task<IEnumerable<RevenuePerBucketDto>> GetRevenuePerDayAsync(DateTime? from, DateTime? to, CancellationToken ct)
        {
            var (fromUtc, toUtc) = Range(from, to);
            var rows = await _uow.Bookings.GetRevenuePerDayAsync(fromUtc, toUtc, ct);
            return rows.Select(r => new RevenuePerBucketDto { Bucket = r.Bucket, Revenue = r.Revenue });
        }

        public async Task<IEnumerable<TopItemDto>> GetTopActivitiesAsync(DateTime? from, DateTime? to, int take, CancellationToken ct)
        {
            var (fromUtc, toUtc) = Range(from, to);
            take = Math.Clamp(take, 1, 50);
            var items = await _uow.Bookings.GetTopActivitiesAsync(fromUtc, toUtc, take, ct);
            return items.Select(i => new TopItemDto { Id = i.Id, Name = i.Name, Count = i.Count });
        }

        public async Task<IEnumerable<TopItemDto>> GetTopPlacesAsync(DateTime? from, DateTime? to, int take, CancellationToken ct)
        {
            var (fromUtc, toUtc) = Range(from, to);
            take = Math.Clamp(take, 1, 50);
            var items = await _uow.Bookings.GetTopPlacesAsync(fromUtc, toUtc, take, ct);
            return items.Select(i => new TopItemDto { Id = i.Id, Name = i.Name, Count = i.Count });
        }

        public async Task<IEnumerable<TopItemDto>> GetBookingsByCategoryAsync(DateTime? from, DateTime? to, CancellationToken ct)
        {
            var (fromUtc, toUtc) = Range(from, to);
            var items = await _uow.Bookings.GetByCategoryAsync(fromUtc, toUtc, ct);
            return items.Select(i => new TopItemDto { Id = i.Id, Name = i.Name, Count = i.Count });
        }
    }
}