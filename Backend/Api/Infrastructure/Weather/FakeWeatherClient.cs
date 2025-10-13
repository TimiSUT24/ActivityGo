using Application.Weather.DTO;
using Application.Weather.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Weather
{
    public sealed class FakeWeatherClient : IWeatherClient
    {
        public Task<WeatherForecastDto> GetAsync(double lat, double lon, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken)
        {
            var hours = Math.Max(0, (int)Math.Ceiling((endUtc - startUtc).TotalHours));
            var slices = Enumerable.Range(0, hours + 1).Select(i => new WeatherSliceDto
            {
                TimeUtc = startUtc.AddHours(i),
                TemperatureC = Math.Round(12 + 5 * Math.Sin(i / 3.0), 1),
                WindSpeedMs = 3 + (i % 4),
                RainVolumeMm = i % 5 == 0 ? 0.4 : 0,
                ConditionIconUrl = "04d",
                ConditionText = "partly cloudy",
                Source = "fake"
            }).ToList();

            return Task.FromResult(new WeatherForecastDto
            {
                Latitude = lat,
                Longitude = lon,
                FromUtc = startUtc,
                ToUtc = endUtc,
                Slices = slices
            });
        }
    }
}
