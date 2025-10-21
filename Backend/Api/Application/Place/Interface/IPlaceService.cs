using Application.Place.DTO;

namespace Application.Place.Interface;

public interface IPlaceService
{
    Task<IEnumerable<PlaceReadDto>> GetAllAsync(CancellationToken ct);
    Task<PlaceReadDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PlaceReadDto> CreateAsync(PlaceCreateDto dto, CancellationToken ct);
    Task<bool> UpdateAsync(Guid id, PlaceUpdateDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<bool> SetActiveAsync(Guid id, bool isActive, CancellationToken ct);
    Task<List<GetActivityPlaceDto>> GetPlaceForActivity(Guid id, CancellationToken ct);
}
