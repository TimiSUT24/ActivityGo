using Application.Category.DTO;

namespace Application.Services.Interfaces;
public interface ICategoryService
{
    Task<IReadOnlyList<CategoryReadDto>> GetAllAsync(CancellationToken ct = default);
    Task<CategoryReadDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, CategoryUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default); // hårddelete
    Task<bool> SetActiveAsync(Guid id, bool isActive, CancellationToken ct = default);
}
