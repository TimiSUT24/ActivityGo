using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Weather.DTO
{
    public sealed class WeatherRequest
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public DateTime StartUtc { get; init; }
        public DateTime EndUtc { get; init; }
    }
}
