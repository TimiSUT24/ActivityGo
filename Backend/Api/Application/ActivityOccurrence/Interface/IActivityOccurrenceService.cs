using Application.ActivityOccurrence.DTO;
using Application.ActivityOccurrence.DTO.Request;
using Application.ActivityOccurrence.DTO.Response;
using Domain.Enums;
using System;
using System.Threading;

namespace Application.ActivityOccurrence.Interface
{
    public interface IActivityOccurrenceService
    {
        Task<ActivityOccurrenceDto> AddAsync(CreateActivityOccurenceDto dto, CancellationToken ct);
        Task<IEnumerable<ActivityOccurrenceDto>> GetAllAsync(CancellationToken ct);
        Task<ActivityOccurrenceDto?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<bool> UpdateAsync(UpdateActivityOccurenceDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct);

        // Get occurrences with weather data with optional filters.
        Task<IReadOnlyList<ActivityOccurrenceWeatherDto>> GetOccurrencesWithWeatherAsync(
            DateTime? fromDate, 
            DateTime? toDate,
            Guid? categoryId, 
            Guid? activityId, 
            Guid? placeId,
            EnvironmentType? environment, 
            bool? onlyAvailable,
            int? minAvailable,
            string? freeTextSearch,
            CancellationToken ct);
    }
}
