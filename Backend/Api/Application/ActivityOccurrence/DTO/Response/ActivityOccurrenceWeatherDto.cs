using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ActivityOccurrence.DTO
{
    public sealed record class ActivityOccurrenceWeatherDto
    {
        public Guid Id { get; init; }
        public DateTime StartUtc { get; init; }
        public DateTime EndUtc { get; init; }
        public int EffectiveCapacity { get; init; }
        public string ActivityName { get; init; } = string.Empty;
        public string ActivityDescription { get; init; } = string.Empty;
        public string PlaceName { get; init; } = string.Empty;
        public EnvironmentType Environment { get; init; }
        public string? CategoryName { get; init; }
        public int DurationMinutes { get; init; }

        // För att enklare kunna uppdatera lediga platser.
        public int BookedPeople { get; set; }
        public int AvailableCapacity { get; set; }
        // Enrichment efter mapping
        public ActivityWeatherForecastDto? WeatherForecast { get; set; }

        public ActivityOccurrenceWeatherDto() { }
    }

    
}
