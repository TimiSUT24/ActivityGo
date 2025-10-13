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
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public DateTime FromUtc { get; init; }
        public DateTime ToUtc { get; init; }
        public IReadOnlyList<WeatherSliceDto> Slices { get; init; } = Array.Empty<WeatherSliceDto>();
    }

    
}