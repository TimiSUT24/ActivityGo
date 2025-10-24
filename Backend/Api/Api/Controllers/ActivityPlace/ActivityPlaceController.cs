using Application.ActivityPlace.DTO.Request;
using Application.ActivityPlace.DTO.Response;
using Application.ActivityPlace.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.ActivityPlace
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ActivityPlaceController : ControllerBase
    {
        private readonly IActivityPlaceService _service;
        public ActivityPlaceController(IActivityPlaceService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> CreateActivityPlace([FromBody] CreateActivityPlaceDto dto, CancellationToken ct)
        {
            await _service.AddAsync(dto, ct);
            return StatusCode(StatusCodes.Status201Created);
        }

        [AllowAnonymous]
        [HttpGet("{activityId:guid}/places")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<GetActivityPlaceDto>>> GetPlacesByActivityId(Guid activityId, CancellationToken ct)
        {
            var places = await _service.GetPlaceForActivity(activityId, ct);
            return Ok(places);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<GetAllActivityPlaceDto>>> GetAllActivityPlaces(CancellationToken ct)
        {
            var places = await _service.GetAllAsync(ct);
            return Ok(places);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateActivityPlace([FromBody] CreateActivityPlaceDto dto, CancellationToken ct)
        {
            var updated = await _service.UpdateAsync(dto, ct);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteActivityPlace([FromBody] CreateActivityPlaceDto dto, CancellationToken ct)
        {
            var deleted = await _service.DeleteAsync(dto, ct);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
