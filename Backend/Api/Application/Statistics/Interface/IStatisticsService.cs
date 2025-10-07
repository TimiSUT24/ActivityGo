using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Statistics.DTO;

namespace Application.Statistics.Interface
{
    public interface IStatisticsService
    {
        Task<SummaryDto> GetSummaryAsync(DateTime? from, DateTime? to, CancellationToken ct);
        Task<IEnumerable<BookingsPerBucketDto>> GetBookingsPerDayAsync(DateTime? from, DateTime? to, CancellationToken ct);
        Task<IEnumerable<RevenuePerBucketDto>> GetRevenuePerDayAsync(DateTime? from, DateTime? to, CancellationToken ct);
        Task<IEnumerable<TopItemDto>> GetTopActivitiesAsync(DateTime? from, DateTime? to, int take, CancellationToken ct);
        Task<IEnumerable<TopItemDto>> GetTopPlacesAsync(DateTime? from, DateTime? to, int take, CancellationToken ct);
        Task<IEnumerable<TopItemDto>> GetBookingsByCategoryAsync(DateTime? from, DateTime? to, CancellationToken ct);
    }
}
