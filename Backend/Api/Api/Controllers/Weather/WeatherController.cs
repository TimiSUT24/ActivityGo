using Application.Weather.DTO;
using Application.Weather.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Weather
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] WeatherRequest query, CancellationToken cancellationToken)
        {
            var result = await _weatherService.GetAsync(
                query.Latitude, query.Longitude, query.StartUtc, query.EndUtc, cancellationToken);
            return Ok(result);
        }
    }
}
