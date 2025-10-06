using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public class IWeatherClient
    {
        Task<WeatherForecastDto> GetAsync(
            double lat, double lon, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken);
    }
}
