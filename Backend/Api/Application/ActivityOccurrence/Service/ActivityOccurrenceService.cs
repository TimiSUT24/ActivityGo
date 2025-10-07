using Application.ActivityOccurrence.DTO.Request;
using Application.ActivityOccurrence.DTO.Response;
using Application.ActivityOccurrence.Interface;
using AutoMapper;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ActivityOccurrence.Service
{
    public class ActivityOccurrenceService : IActivityOccurrenceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IWeatherService _weatherService;

        public ActivityOccurrenceService(IUnitOfWork uow, IMapper mapper, IWeatherService weatherService)
        {
            _uow = uow;
            _mapper = mapper;
            _weatherService = weatherService;
        }

        public async Task<ActivityOccurrenceDto> AddAsync(CreateActivityOccurenceDto dto, CancellationToken ct)
        {
            var entity = _mapper.Map<Domain.Models.ActivityOccurrence>(dto);
            await _uow.Occurrences.AddAsync(entity, ct);

            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<ActivityOccurrenceDto>(entity);
        }

        public async Task<IEnumerable<ActivityOccurrenceDto>> GetAllAsync(CancellationToken ct)
        {
            var entities = await _uow.Occurrences.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<ActivityOccurrenceDto>>(entities);
        }

        public async Task<ActivityOccurrenceDto?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var entity = await _uow.Occurrences.GetByIdAsync(id, ct);
            
            if(entity == null)
            {
                return null;
            }            
            return _mapper.Map<ActivityOccurrenceDto>(entity);
        }

        public async Task<bool> UpdateAsync(UpdateActivityOccurenceDto dto, CancellationToken ct)
        {
            var entity = await _uow.Occurrences.GetByIdAsync(dto.Id, ct);
            if (entity == null)
            {
                return false;
            }

            _mapper.Map(dto, entity);
            _uow.Occurrences.Update(entity);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _uow.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        {
            var entity = await _uow.Occurrences.GetByIdAsync(id, ct);
            if (entity == null)
            {
                return false;
            }

            _uow.Occurrences.Delete(entity);
            await _uow.SaveChangesAsync(ct);
            return true;
        }

        /*=============ActivityOccurrence + Weather=============*/

        public async Task<IReadOnlyList<ActivityOccurrenceWeatherDto>> GetOccurrencesWithWeatherAsync(
            DateTime fromDate, DateTime toDate, CancellationToken ct)
        {

        }
    }
}
