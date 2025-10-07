using Application.ActivityOccurrence.DTO.Request;
using Application.ActivityOccurrence.DTO.Response;
using Application.ActivityOccurrence.DTO;

namespace Application.ActivityOccurrence.Interface
{
    public interface IActivityOccurrenceService
    {
        Task<ActivityOccurrenceDto> AddAsync(CreateActivityOccurenceDto dto, CancellationToken ct);
        Task<IEnumerable<ActivityOccurrenceDto>> GetAllAsync(CancellationToken ct);
        Task<ActivityOccurrenceDto?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<bool> UpdateAsync(UpdateActivityOccurenceDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct);

        // New method to get occurrences with weather data
        Task<IReadOnlyList<ActivityOccurrenceWeatherDto>> GetOccurrencesWithWeatherAsync(
            DateTime fromDate, DateTime toDate, CancellationToken ct);
    }
}
