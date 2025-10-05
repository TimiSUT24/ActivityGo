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

    }
}
