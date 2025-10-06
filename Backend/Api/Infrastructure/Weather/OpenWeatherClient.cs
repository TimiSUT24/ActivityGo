using Application.Weather.DTO;
using Application.Weather.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Infrastructure.Weather
{
    public class OpenWeatherClient : IWeatherClient
    {
        private readonly HttpClient _http;
        private readonly OpenWeatherOptions _options;

        public OpenWeatherClient(HttpClient http, IOptions<OpenWeatherOptions> options)
        {
            _http = http;
            _options = options.Value;
        }


        public async Task<WeatherForecastDto> GetAsync(double lat, double lon, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken)
        {
            var url = $"{_options.BaseUrl}/forecast?lat={lat}&lon={lon}&units=metric&appid={_options.ApiKey}";
            var ow = await _http.GetFromJsonAsync<OwForecast>(url, cancellationToken)
                     ?? throw new InvalidOperationException("Weather provider returned no data");

            var slices = ow.list
                .Where(x => x.dt_txt != null)
                .Select(x => new
                {
                    t = DateTime.SpecifyKind(DateTime.Parse(x.dt_txt!, System.Globalization.CultureInfo.InvariantCulture), DateTimeKind.Utc),
                    x
                })
                .Where(x => x.t >= startUtc && x.t <= endUtc)
                .Select(x => new WeatherSliceDto
                {
                    TimeUtc = x.t,
                    TemperatureC = x.x.main.temp,
                    WindSpeedMs = x.x.wind.speed,
                    rainVolumeMm = x.x.rain?.GetValueOrDefault("3h") ?? 0.0,
                    ConditionIconUrl = x.x.weather.FirstOrDefault()?.icon ?? "",
                    ConditionText = x.x.weather.FirstOrDefault()?.description ?? "",
                    Source = "openweather"
                })
                .ToList();

            return new WeatherForecastDto
            {
                Latitude = lat,
                Longitude = lon,
                FromUtc = startUtc,
                ToUtc = endUtc,
                Slices = slices
            };
        }

        // Minimala modeller för leverantörens JSON
        private sealed class OwForecast { public List<Item> list { get; set; } = new(); }
        private sealed class Item
        {
            public string? dt_txt { get; set; }
            public Main main { get; set; } = new();
            public Wind wind { get; set; } = new();
            public List<W> weather { get; set; } = new();
            public Dictionary<string, double>? rain { get; set; }
        }
        private sealed class Main { public double temp { get; set; } }
        private sealed class Wind { public double speed { get; set; } }
        private sealed class W { public string icon { get; set; } = ""; public string description { get; set; } = ""; }
    }
}
