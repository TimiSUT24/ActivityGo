using Application.ActivityOccurrence.DTO.Request;
using Application.ActivityOccurrence.DTO.Response;
using Application.ActivityOccurrence.Interface;
using Application.Weather.DTO;
using Application.Weather.Interface;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces;
using Application.Weather.DTO;

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

        public async Task<IReadOnlyList<ActivityOccurrenceWeather>> GetOccurrencesWithWeatherAsync(
            DateTime fromDate, DateTime toDate, CancellationToken ct)
        {
            var occurences = await _uow.GetOccurenceBetweenDatesWithPlaceAndActivityAsync(fromDate, toDate, ct);

            var dtos = _mapper.Map<IReadOnlyList<ActivityOccurrenceWeather>>(occurences);

            var enrichmentTasks = new List<Task>();

            foreach (var dto in dtos.Where(d => d.Environment == EnvironmentType.Outdoor))
            {
                var domainEntity = occurences.First(o => o.Id == dto.Id);

                if (domainEntity.Place.Latitude.HasValue && domainEntity.Place.Longitude.HasValue)
                {
                    enrichmentTasks.Add(
                        EnrichWithWeatherAsync(dto, domainEntity, ct));
                }
            }
            await Task.WhenAll(enrichmentTasks);

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
            catch (Exception)
            {
                // If something goes wrong, we dont provide weather data
                dto.WeatherForecast = null;
            }
        }
    }
}
