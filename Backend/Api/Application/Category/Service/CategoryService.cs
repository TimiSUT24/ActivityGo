using Application.Category.DTO;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;
public sealed class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CategoryReadDto>> GetAllAsync(CancellationToken ct = default)
    {
        // AsNoTracking redan i repo.GetAllAsync(); projicera gärna direkt
        var query = _uow.Categories.Query().AsNoTracking();
        return await query
            .ProjectTo<CategoryReadDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<CategoryReadDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _uow.Categories.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<CategoryReadDto>(entity);
    }

    public async Task<Guid> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default)
    {
        // enkel unikhetskoll på namn
        if (await _uow.Categories.ExistsByNameAsync(dto.Name, ct))
            throw new InvalidOperationException($"Category with name '{dto.Name}' already exists.");

        var entity = _mapper.Map<Domain.Models.Category>(dto);
        entity.Id = Guid.NewGuid();

        await _uow.Categories.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, CategoryUpdateDto dto, CancellationToken ct = default)
    {
        var entity = await _uow.Categories.GetByIdAsync(id, ct);
        if (entity is null) return false;

        // om namn ändras – kolla unikhet
        if (!string.IsNullOrWhiteSpace(dto.Name) && !dto.Name.Equals(entity.Name, StringComparison.Ordinal))
        {
            if (await _uow.Categories.ExistsByNameAsync(dto.Name, ct))
                throw new InvalidOperationException($"Category with name '{dto.Name}' already exists.");
        }

        _mapper.Map(dto, entity);
        _uow.Categories.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _uow.Categories.GetByIdAsync(id, ct);
        if (entity is null) return false;

        _uow.Categories.Delete(entity);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SoftDeactivateAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _uow.Categories.GetByIdAsync(id, ct);
        if (entity is null) return false;

        entity.IsActive = false;
        _uow.Categories.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
