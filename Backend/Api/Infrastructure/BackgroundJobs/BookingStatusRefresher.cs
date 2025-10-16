using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;
using Infrastructure.Persistence; // AppDbContext

namespace Infrastructure.BackgroundJobs
{
    public class BookingStatusRefresher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BookingStatusRefresher> _logger;
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5); // justera vid behov

        public BookingStatusRefresher(IServiceScopeFactory scopeFactory, ILogger<BookingStatusRefresher> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // liten jitter vid uppstart så vi inte prickar exakt på minutgräns
            var rnd = new Random();
            await Task.Delay(TimeSpan.FromSeconds(rnd.Next(3, 12)), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var affected = await db.Bookings
                        .Where(b => b.Status == BookingStatus.Booked)
                        .Where(b => db.ActivityOccurrences
                            .Any(ao => ao.Id == b.ActivityOccurrenceId && ao.EndUtc < now))
                        .ExecuteUpdateAsync(s => s.SetProperty(b => b.Status, BookingStatus.Completed), stoppingToken);

                    if (affected > 0)
                        _logger.LogInformation("[BookingStatusRefresher] Marked {Count} bookings as Completed at {Now:u}", affected, now);
                }
                catch (OperationCanceledException)
                {
                    // normal shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[BookingStatusRefresher] Error while updating booking statuses.");
                }

                try
                {
                    await Task.Delay(Interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // normal shutdown
                }
            }
        }
    }
}
