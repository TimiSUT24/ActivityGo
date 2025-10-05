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
    }
}
