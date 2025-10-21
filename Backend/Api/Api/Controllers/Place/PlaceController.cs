using Application.Place.DTO;
using Application.Place.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // lås CRUD till Admins
public class PlaceController : ControllerBase
{
    private readonly IPlaceService _svc;
    public PlaceController(IPlaceService svc) => _svc = svc;

    [HttpGet, AllowAnonymous] // valfritt: öppna för publik listning
    public async Task<ActionResult<IEnumerable<PlaceReadDto>>> GetAll(CancellationToken ct)
        => Ok(await _svc.GetAllAsync(ct));

    [HttpGet("{id:guid}"), AllowAnonymous]
    public async Task<ActionResult<PlaceReadDto>> GetById(Guid id, CancellationToken ct)
    {
        var res = await _svc.GetByIdAsync(id, ct);
        return res is null ? NotFound() : Ok(res);
    }

    [HttpPost]
    public async Task<ActionResult<PlaceReadDto>> Create([FromBody] PlaceCreateDto dto, CancellationToken ct)
    {
        var created = await _svc.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PlaceUpdateDto dto, CancellationToken ct)
    {
        var ok = await _svc.UpdateAsync(id, dto, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _svc.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpPatch("{id:guid}/active/{isActive:bool}")]
    public async Task<IActionResult> SetActive(Guid id, bool isActive, CancellationToken ct)
    {
        var ok = await _svc.SetActiveAsync(id, isActive, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("{activityId}/places")]
    public async Task<ActionResult<List<GetActivityPlaceDto>>> GetPlacesByActivityId(Guid activityId, CancellationToken ct)
    {
        var places = await _svc.GetPlaceForActivity(activityId, ct);
        return Ok(places);
    }
}
