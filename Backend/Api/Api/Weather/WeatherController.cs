using Application.Weather.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Weather
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("LocalWeather")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetWeather(double lat, double lon, CancellationToken ct)
        {
            var nowUtc = DateTime.UtcNow;
            var endUtc = nowUtc.AddHours(12);
            var result = await _weatherService.GetAsync(lat, lon, nowUtc, endUtc, ct);
            return Ok(result); 
        }
    }
}
