using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityOccurrence.DTO
{
    public sealed record ActivityWeatherForecastDto
    (
        // General forecast info
        DateTime TimeUtc, // Time of the forecast (nearest to activity start)
        double TemperatureC,
        double WindSpeedMs,
        string ConditionText,
        string ConditionIconUrl,
        double RainVolumeMm
    );
}
