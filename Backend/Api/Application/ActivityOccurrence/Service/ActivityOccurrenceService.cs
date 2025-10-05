using Application.ActivityOccurrence.DTO.Request;
using Application.ActivityOccurrence.DTO.Response;
using Application.ActivityOccurrence.Interface;
using AutoMapper;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityOccurrence.Service
{
    public class ActivityOccurrenceService : IActivityOccurrenceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ActivityOccurrenceService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ActivityOccurrenceDto> AddAsync(CreateActivityOccurenceDto dto, CancellationToken ct)
        {
            var entity = _mapper.Map<Domain.Models.ActivityOccurrence>(dto);
            await _uow.Occurrences.AddAsync(entity, ct);

            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<ActivityOccurrenceDto>(entity);
        }


    }
}
