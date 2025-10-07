using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityOccurrence.DTO
{
    public class ActivityWeatherForecastDto
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
