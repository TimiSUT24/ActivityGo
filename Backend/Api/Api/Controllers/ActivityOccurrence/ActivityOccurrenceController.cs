using Application.ActivityOccurrence.DTO;
using Application.ActivityOccurrence.DTO.Request;
using Application.ActivityOccurrence.DTO.Response;
using Application.ActivityOccurrence.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.ActivityOccurrence
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ActivityOccurrenceController : ControllerBase
    {
        private readonly IActivityOccurrenceService _activityOccurrenceService;

        public ActivityOccurrenceController(IActivityOccurrenceService activityOccurrenceService)
        {
            _activityOccurrenceService = activityOccurrenceService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ActivityOccurrenceDto>> Create([FromBody] CreateActivityOccurenceDto dto, CancellationToken ct = default)
        {
            var created = await _activityOccurrenceService.AddAsync(dto, ct);
            return CreatedAtAction(nameof(Create), new { id = created.Id }, created);
        }
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ActivityOccurrenceDto>>> GetAll(CancellationToken ct = default)
        {
            var occurrences = await _activityOccurrenceService.GetAllAsync(ct);
            if (occurrences == null || !occurrences.Any())
            {
                return NotFound();
            }
            return Ok(occurrences);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ActivityOccurrenceDto>> GetById(Guid id, CancellationToken ct = default)
        {
            var occurrence = await _activityOccurrenceService.GetByIdAsync(id, ct);
            if (occurrence == null)
            {
                return NotFound();
            }
            return Ok(occurrence);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update([FromBody] UpdateActivityOccurenceDto dto, CancellationToken ct = default)
        {
            var updated = await _activityOccurrenceService.UpdateAsync(dto, ct);
            if (!updated)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct = default)
        {
            var deleted = await _activityOccurrenceService.DeleteAsync(id, ct);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("with-weather")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ActivityOccurrenceWeatherDto>>> GetOccurrencesWithWeather(
            [FromQuery] OccurencyQuery q, 
            CancellationToken ct = default)
        {

            var result = await _activityOccurrenceService.GetOccurrencesWithWeatherAsync(
                 q.FromDate, 
                 q.ToDate,
                 q.CategoryId, 
                 q.ActivityId, 
                 q.PlaceId,
                 q.Environment, 
                 q.OnlyAvailable,
                 q.MinAvailable,
                 q.FreeTextSearch,
                 ct);
            return Ok(result);
        }
    }
}
