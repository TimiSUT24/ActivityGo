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
            // OpenWeather kräver lat/lon med punkt som decimalavskiljare, oavsett kultur därför fick jag tvinga den till det då "," är standard i sv-SE
            //Detta löste inte problemet så vi letar vidare.
            var latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var lonStr = lon.ToString(System.Globalization.CultureInfo.InvariantCulture);

            var url = $"{_options.BaseUrl}/onecall?lat={latStr}&lon={lonStr}&units=metric&appid={_options.ApiKey}";
            //var url = $"{_options.BaseUrl}/onecall?lat={lat}&lon={lon}&units=metric&appid={_options.ApiKey}";

            var ow = await _http.GetFromJsonAsync<OwForecast>(url, cancellationToken)
                     ?? throw new InvalidOperationException("Weather provider returned no data");

            var slices = ow.hourly
                .Select(x => new
                {
                    t = DateTimeOffset.FromUnixTimeSeconds(x.dt).UtcDateTime,
                    x
                })
                .Where(x => x.t >= startUtc && x.t <= endUtc)
                .Select(x => new WeatherSliceDto
                {
                    TimeUtc = x.t,
                    TemperatureC = x.x.temp,
                    WindSpeedMs = x.x.wind_speed,
                    RainVolumeMm = x.x.rain?.OneHour ?? 0.0,
                    ConditionIconUrl = x.x.weather.FirstOrDefault()?.icon ?? "",
                    ConditionText = x.x.weather.FirstOrDefault()?.description ?? "",
                    Source = "openweather3.0"
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
        private sealed class OwForecast
        {
            public List<HourlyItem> hourly { get; set; } = new();
        }

        // Representerar ett timvis prognos-objekt
        private sealed class HourlyItem
        {
            public long dt { get; set; } // Unix-tid (kräver konvertering)
            public double temp { get; set; }
            public double wind_speed { get; set; }
            public List<W> weather { get; set; } = new();
            public RainData? rain { get; set; }
        }

        private sealed class RainData
        {
            [System.Text.Json.Serialization.JsonPropertyName("1h")]
            public double? OneHour { get; set; }
        }

        private sealed class W
        {
            public string icon { get; set; } = "";
            public string description { get; set; } = "";
        }
    }
}
