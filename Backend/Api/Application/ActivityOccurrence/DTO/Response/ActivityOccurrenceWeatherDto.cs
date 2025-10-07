using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityOccurrence.DTO
{
    public sealed class ActivityOccurrenceWeatherDto
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

    
}
