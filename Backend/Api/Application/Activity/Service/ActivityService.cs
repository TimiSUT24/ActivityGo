using Application.Activity.DTO.Request;
using Application.Activity.DTO.Response;
using Application.Activity.Interface;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Activity.Service
{
    public class ActivityService : IActivityService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ActivityService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ActivityResponse> CreateAsync(ActivityCreateRequest request, CancellationToken cancellationToken)
        {
            if(await _uow.Activities.ExistsByNameAsync(request.Name, cancellationToken))
                throw new InvalidOperationException($"An activity with the name '{request.Name}' already exists.");

            var entity = _mapper.Map<SportActivity>(request);
            await _uow.Activities.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ActivityResponse>(entity);
        }
        public async Task<ActivityResponse?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _uow.Activities.GetByIdAsync(id, cancellationToken);
            return entity == null ? null : _mapper.Map<ActivityResponse>(entity);

        }

        public async Task<IReadOnlyList<ActivityResponse>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken)
        {
            var q = includeInactive 
                ? _uow.Activities.Query() 
                : _uow.Activities.Query()
                .Where(a => a.IsActive);

            var list = await q.OrderBy(a => a.Name).ToListAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<ActivityResponse>>(list);
        }

        public async Task<bool> UpdateAsync(Guid id, ActivityUpdateRequest request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Activities.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                return false;
            }

            _mapper.Map(request, entity);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _uow.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _uow.Activities.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                return false;
            }
            _uow.Activities.Delete(entity);
            await _uow.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
