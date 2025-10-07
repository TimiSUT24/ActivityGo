using Domain.Enums;
using Domain.Models;

namespace Domain.Interfaces;

public interface IActivityOccurrenceRepository : IGenericRepository<ActivityOccurrence>
{
    // används av StatisticsService för beläggning
    Task<IReadOnlyList<OccurrenceUtilItem>> GetUtilizationItemsAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct);
   
    // New method for weather integration in ActivityOccurence, eager loaded place & activity.
    Task<IReadOnlyList<ActivityOccurrence>> GetOccurrencesBetweenDatesWithPlaceAndActivityAsync(DateTime fromDate, DateTime toDate, CancellationToken ct);

    Task<IReadOnlyList<ActivityOccurrence>> GetBetweenDatesFilteredAsync(
        DateTime fromDate, DateTime toDate,
        Guid? categoryId, Guid? activityId, Guid? placeId,
        EnvironmentType? environment, bool? onlyAvailable,
        CancellationToken ct);
}

public class OccurrenceUtilItem
{
    public Guid OccurrenceId { get; set; }
    public int Capacity { get; set; }
    public int NonCancelledBookings { get; set; }
}