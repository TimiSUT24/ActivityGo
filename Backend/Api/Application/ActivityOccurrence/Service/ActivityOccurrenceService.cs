using Application.ActivityOccurrence.DTO;
using Application.ActivityOccurrence.DTO.Request;
using Application.ActivityOccurrence.DTO.Response;
using Application.ActivityOccurrence.Interface;
using Application.Weather.DTO;
using Application.Weather.Interface;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

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

            if (entity == null)
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
            DateTime? fromDate, 
            DateTime? toDate,
            Guid? categoryId, 
            Guid? activityId, 
            Guid? placeId,
            EnvironmentType? environment, 
            bool? onlyAvailable,
            int? minAvailable,
            CancellationToken ct)
        {
            var (start, end) = Normalize(fromDate, toDate);

            var repo = (IActivityOccurrenceRepository)_uow.Occurrences;
            var occurrences = await repo.GetBetweenDatesFilteredAsync(
                start, end, categoryId, activityId, placeId, environment, onlyAvailable, ct);

            var dtos = _mapper.Map<IReadOnlyList<ActivityOccurrenceWeatherDto>>(occurrences);
            // Uträkning för antal platser kvar.
            var bookedById = occurrences.ToDictionary(
                o => o.Id,
                o => o.Bookings
                    .Where(b => b.Status == BookingStatus.Booked)
                    .Sum(b => b.PeopleCount)
                    );
            foreach (var d in dtos)
            {
                d.BookedPeople = bookedById.TryGetValue(d.Id, out var sum) ? sum : 0;
                d.AvailableCapacity = Math.Max(0, d.EffectiveCapacity - d.BookedPeople);
            }
            // Filtrering på minsta antal platser kvar
            if (minAvailable.HasValue)
            {
                dtos = dtos.Where(d => d.AvailableCapacity >= minAvailable.Value).ToList();
            }

            var tasks = new List<Task>(dtos.Count);
            foreach (var dto in dtos.Where(d => d.Environment == EnvironmentType.Outdoor))
            {
                var domain = occurrences.First(o => o.Id == dto.Id);
                if (domain.Place.Latitude.HasValue && domain.Place.Longitude.HasValue)
                    tasks.Add(EnrichWithWeatherAsync(dto, domain, ct));
            }

            try { await Task.WhenAll(tasks); }
            catch (OperationCanceledException) { throw; }
            catch { /* ignorera enrichment-fel */ }
            return dtos;
        }

        private async Task EnrichWithWeatherAsync(
            ActivityOccurrenceWeatherDto dto, Domain.Models.ActivityOccurrence domain, CancellationToken ct)
        {
            double lat = domain.Place.Latitude!.Value;
            double lon = domain.Place.Longitude!.Value;
            DateTime start = domain.StartUtc;

            try
            {
                // Uses the cached weather service to get the forecast
                var forecast = await _weatherService.GetAsync(lat, lon, start, domain.EndUtc, ct);

                // Sets the weather forecast to the slice closest to the start time of the activity
                var relevantSlice = forecast.Slices
                    .OrderBy(s => Math.Abs((s.TimeUtc - start).TotalMinutes))
                    .FirstOrDefault();

                if (relevantSlice != null)
                {
                    dto.WeatherForecast = _mapper.Map<ActivityWeatherForecastDto>(relevantSlice);
                }
            }
            catch (OperationCanceledException) { throw; }
            catch
            {
                // If something goes wrong, we dont provide weather data
                dto.WeatherForecast = null;
            }
        }

        /*============= Hjälpare: datum-normalisering =============*/
        private static (DateTime start, DateTime end) Normalize(DateTime? from, DateTime? to)
        {   // Normalizes "from" and "to" to UTC, with these rules:
            DateTime ToUtcAssumeLocal(DateTime dt) =>
                dt.Kind switch
                {
                    DateTimeKind.Utc => dt,
                    DateTimeKind.Local => dt.ToUniversalTime(),
                    _ => DateTime.SpecifyKind(dt, DateTimeKind.Local).ToUniversalTime()
                };

            var start = from.HasValue
                ? ToUtcAssumeLocal(from.Value.TimeOfDay == TimeSpan.Zero
                    ? from.Value.Date // dygnsstart
                    : from.Value)
                : DateTime.UtcNow.AddDays(-7);

            DateTime end;
            if (to.HasValue)
            {
                var t = to.Value;
                end = ToUtcAssumeLocal(t);
                if (t.TimeOfDay == TimeSpan.Zero)
                    end = ToUtcAssumeLocal(t.Date).AddDays(1);
            }
            else
            {
                // If no "to" is given, we default to one day after "from"
                end = from.HasValue && from.Value.TimeOfDay == TimeSpan.Zero
                    ? ToUtcAssumeLocal(from.Value.Date).AddDays(1)
                    : start.AddDays(1);
            }

            return (start, end);
        }
    }
}
