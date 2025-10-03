// Api/Controllers/Admin/StatisticsController.cs
using Application.Statistics.DTO;
using Application.Statistics.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // endast admin
    [Produces("application/json")]
    public sealed class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _svc;
        public StatisticsController(IStatisticsService svc) => _svc = svc;

        /// <summary>Översikt (KPI) för valt intervall. Default: senaste 30 dagarna.</summary>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(SummaryDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SummaryDto>> Summary([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
            => Ok(await _svc.GetSummaryAsync(from, to, ct));

        /// <summary>Antal bokningar per dag.</summary>
        [HttpGet("bookings-per-day")]
        [ProducesResponseType(typeof(IEnumerable<BookingsPerBucketDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookingsPerBucketDto>>> BookingsPerDay([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
            => Ok(await _svc.GetBookingsPerDayAsync(from, to, ct));

        /// <summary>Intäkt per dag (Completed).</summary>
        [HttpGet("revenue-per-day")]
        [ProducesResponseType(typeof(IEnumerable<RevenuePerBucketDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RevenuePerBucketDto>>> RevenuePerDay([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
            => Ok(await _svc.GetRevenuePerDayAsync(from, to, ct));

        /// <summary>Toppaktiviteter efter antal bokningar.</summary>
        [HttpGet("top-activities")]
        [ProducesResponseType(typeof(IEnumerable<TopItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TopItemDto>>> TopActivities([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int take = 5, CancellationToken ct = default)
        {
            take = Math.Clamp(take, 1, 50);
            var data = await _svc.GetTopActivitiesAsync(from, to, take, ct);
            return Ok(data);
        }

        /// <summary>Topp-platser efter antal bokningar.</summary>
        [HttpGet("top-places")]
        [ProducesResponseType(typeof(IEnumerable<TopItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TopItemDto>>> TopPlaces([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int take = 5, CancellationToken ct = default)
        {
            take = Math.Clamp(take, 1, 50);
            var data = await _svc.GetTopPlacesAsync(from, to, take, ct);
            return Ok(data);
        }

        /// <summary>Bokningar grupperade per kategori.</summary>
        [HttpGet("bookings-by-category")]
        [ProducesResponseType(typeof(IEnumerable<TopItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TopItemDto>>> ByCategory([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
            => Ok(await _svc.GetBookingsByCategoryAsync(from, to, ct));
    }
}