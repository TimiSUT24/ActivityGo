using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Weather.DTO
{
    public sealed class WeatherForecastDto
    {
        public double Latitude {get; init; }
        public double Longitude { get; init; }
        public DateTime FromUtc { get; init; }
        public DateTime ToUtc { get; init; }
        public IReadOnlyList<WeatherSliceDto> Slices { get; init; } = Array.Empty<WeatherSliceDto>();
    }

    public sealed class WeatherSliceDto
    {
        public DateTime TimeUtc { get; init; }
        public double TemperatureC { get; init; }
        public double WindSpeedMs { get; init; }
        public int HumidityPercent { get; init; }
        public float rainVolumeMm { get; init; }
        public string ConditionText { get; init; } = string.Empty;
        public string ConditionIconUrl { get; init; } = string.Empty;
        public string Source { get; init; } = "openweather";
    }



}
