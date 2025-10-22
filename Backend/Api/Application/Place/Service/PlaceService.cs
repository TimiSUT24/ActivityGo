using Application.ActivityPlace.DTO.Response;
using Application.Place.DTO;
using Application.Place.Interface;
using AutoMapper;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Place.Service;

public class PlaceService : IPlaceService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<PlaceCreateDto> _createValidator;
    private readonly IValidator<PlaceUpdateDto> _updateValidator;

    public PlaceService(
        IUnitOfWork uow,
        IMapper mapper,
        IValidator<PlaceCreateDto> createValidator,
        IValidator<PlaceUpdateDto> updateValidator)
    {
        _uow = uow;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<PlaceReadDto>> GetAllAsync(CancellationToken ct)
    {
        var list = await _uow.Places.Query().ToListAsync(ct);
        return _mapper.Map<IEnumerable<PlaceReadDto>>(list);
    }

    public async Task<PlaceReadDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var entity = await _uow.Places.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<PlaceReadDto>(entity);
    }

    public async Task<PlaceReadDto> CreateAsync(PlaceCreateDto dto, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(dto, ct);

        if (await _uow.Places.ExistsByNameAsync(dto.Name, ct))
            throw new InvalidOperationException($"Place med namn '{dto.Name}' finns redan.");

        var entity = _mapper.Map<Domain.Models.Place>(dto);
        await _uow.Places.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        var created = await _uow.Places.GetByIdAsync(entity.Id, ct)!;
        return _mapper.Map<PlaceReadDto>(created);
    }

    public async Task<bool> UpdateAsync(Guid id, PlaceUpdateDto dto, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(dto, ct);

        var entity = await _uow.Places.GetByIdAsync(id, ct);
        if (entity is null) return false;

        if (!string.Equals(entity.Name, dto.Name, StringComparison.OrdinalIgnoreCase)
            && await _uow.Places.ExistsByNameAsync(dto.Name, ct))
            throw new InvalidOperationException($"Place med namn '{dto.Name}' finns redan.");

        _mapper.Map(dto, entity);
        _uow.Places.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await _uow.Places.GetByIdAsync(id, ct);
        if (entity is null) return false;

        _uow.Places.Delete(entity);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SetActiveAsync(Guid id, bool isActive, CancellationToken ct)
    {
        var entity = await _uow.Places.GetByIdAsync(id, ct);
        if (entity is null) return false;

        entity.IsActive = isActive;
        _uow.Places.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
