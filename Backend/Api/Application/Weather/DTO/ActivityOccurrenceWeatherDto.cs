using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Weather.DTO
{
    public class ActivityOccurrenceWeather
    {
        public Guid Id { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public int EffectiveCapacity { get; set; }


        // Activity and Place details for context
        public string ActivityName { get; set; } = string.Empty;
        public string PlaceName { get; set; } = string.Empty;
        public EnvironmentType Environment { get; set; }

        // Weather details
        public ActivityWeatherForecastDto? WeatherForecast { get; set; }
    }

    public sealed class ActivityWeatherForecastDto
    {
        // General forecast info
        public DateTime TimeUtc { get; init; } // Time of the forecast (nearest to activity start)
        public double TemperatureC { get; init; }
        public double WindSpeedMs { get; init; }
        public string ConditionText { get; init; } = string.Empty;
        public string ConditionIconUrl { get; init; } = string.Empty;
        public double RainVolumeMm { get; init; }
    }
}
