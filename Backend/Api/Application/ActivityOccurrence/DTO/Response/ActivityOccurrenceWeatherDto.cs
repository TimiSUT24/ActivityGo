using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityOccurrence.DTO
{
    public sealed record ActivityOccurrenceWeatherDto
    (
        Guid Id,
        DateTime StartUtc,
        DateTime EndUtc,
        int EffectiveCapacity,

        // Activity and Place details for context
        string? CategoryName,
        string ActivityName,
        int ActivityDurationMinutes,

        string PlaceName,
        EnvironmentType Environment
    )
    {
        // Weather details
        // Nullable, as not all occurrences will have weather data
        public ActivityWeatherForecastDto? WeatherForecast { get; set; }
    }

    
}
