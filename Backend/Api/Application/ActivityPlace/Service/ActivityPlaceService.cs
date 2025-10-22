using Application.ActivityPlace.DTO.Request;
using Application.ActivityPlace.DTO.Response;
using Application.ActivityPlace.Interface;
using Application.Exceptions;
using AutoMapper;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityPlace.Service
{
    public class ActivityPlaceService : IActivityPlaceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ActivityPlaceService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(CreateActivityPlaceDto dto, CancellationToken ct)
        {
            var isExist = await _uow.ActivityPlaces.AnyAsync(ap => ap.SportActivityId == dto.SportActivityId && ap.PlaceId == dto.PlaceId, ct);
            if (isExist)
            {
                throw new ConflictException("Relation already exists");
            }
            var entity = _mapper.Map<Domain.Models.ActivityPlace>(dto);
            await _uow.ActivityPlaces.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return true;
        }

        public async Task<List<GetActivityPlaceDto>> GetPlaceForActivity(Guid id, CancellationToken ct)
        {
            var activityPlaces = await _uow.ActivityPlaces.GetPlaceForActivityAsync(id, ct);
            if (activityPlaces == null || !activityPlaces.Any())
            {
                throw new ArgumentException($"No places linked to Activity with ID {id}.");
            }
            // Assuming we return the first linked place for simplicity

            return _mapper.Map<List<GetActivityPlaceDto>>(activityPlaces);
        }

        public async Task<IEnumerable<GetAllActivityPlaceDto>> GetAllAsync(CancellationToken ct)
        {
            var list = await _uow.ActivityPlaces.GetAllAsync(ct);
            if(list == null || !list.Any())
            {
                throw new ArgumentException("No ActivityPlace relations found.");
            }
            return _mapper.Map<IEnumerable<GetAllActivityPlaceDto>>(list);
        }

        public async Task<bool> UpdateAsync(CreateActivityPlaceDto dto, CancellationToken ct)
        {

            var currentEntity = await _uow.ActivityPlaces.AnyAsync(ap => ap.SportActivityId == dto.SportActivityId && ap.PlaceId == dto.PlaceId, ct);

            if (currentEntity)
            {
                throw new ConflictException($"ActivityPlace relation between Activity ID '{dto.SportActivityId}' and Place ID '{dto.PlaceId}' already exists.");
            }

            var existing = await _uow.ActivityPlaces
            .FirstOrDefaultAsync(ap => ap.SportActivityId == dto.SportActivityId, ct);

            if (existing == null)
            {
                return false;
            }
                
            // Ta bort gamla platsen
            _uow.ActivityPlaces.Delete(existing);

            // Lägg till ny relation
            var newEntity = new Domain.Models.ActivityPlace
            {
                SportActivityId = dto.SportActivityId,
                PlaceId = dto.PlaceId
            };
            await _uow.ActivityPlaces.AddAsync(newEntity, ct);

            var occurrences = await _uow.Occurrences
                .Query()
                .Where(s => s.ActivityId == dto.SportActivityId)
                .ToListAsync(ct);

            foreach(var occ in occurrences)
            {
                occ.PlaceId = dto.PlaceId;
                _uow.Occurrences.Update(occ);
            }

            await _uow.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(CreateActivityPlaceDto dto, CancellationToken ct)
        {
            var entity = await _uow.ActivityPlaces.FirstOrDefaultAsync(ap => ap.SportActivityId == dto.SportActivityId && ap.PlaceId == dto.PlaceId, ct);
            if(entity == null)
            {
                throw new KeyNotFoundException("Relation does not exist");
            }
            _uow.ActivityPlaces.Delete(entity);
            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }
}
