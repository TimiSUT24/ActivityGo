using Application.Weather.DTO;
using Application.Weather.Interface;
using Application.Weather.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Weather.Service
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherClient _client;
        private readonly IMemoryCache _cache;

        public WeatherService(IWeatherClient client, IMemoryCache cache)
        {
            _client = client;
            _cache = cache;
        }

        private static DateTime RoundHour(DateTime t) => new(t.Year, t.Month, t.Day, t.Hour, 0, 0, DateTimeKind.Utc);

        public Task<WeatherForecastDto> GetAsync(double lat, double lon, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken)
        {
            var a = RoundHour(startUtc);
            var b = RoundHour(endUtc);
            var key = $"weather-{lat:F4}-{lon:F4}-{a:o}-{b:o}";
            
            return _cache.GetOrCreateAsync(key, e =>
            {
               e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return _client.GetAsync(lat, lon, a, b, cancellationToken);
            })!;
        }
    }
}
