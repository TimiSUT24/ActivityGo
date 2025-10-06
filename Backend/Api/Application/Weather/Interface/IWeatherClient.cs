using Application.Weather.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Weather.Interfaces
{
    public interface IWeatherClient
    {
        Task<WeatherForecastDto> GetAsync(
            double lat, double lon, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken);
    }
}
