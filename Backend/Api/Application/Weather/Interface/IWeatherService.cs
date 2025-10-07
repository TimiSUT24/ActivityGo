using Application.Weather.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Weather.Interface
{
    public interface IWeatherService
    {
        Task<WeatherForecastDto> GetAsync(
            double lat, double lon, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken);
    }
}
