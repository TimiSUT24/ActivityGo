using Application.Booking.DTO;
using Application.Booking.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // alla inloggade användare
public class BookingController : ControllerBase
{
    private readonly IBookingService _svc;
    public BookingController(IBookingService svc) => _svc = svc;

    private string UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("Ingen inloggad användare.");

    [HttpGet("me")]
    public async Task<ActionResult<IEnumerable<BookingReadDto>>> GetMine([FromQuery] string? scope, CancellationToken ct)
        => Ok(await _svc.GetMineAsync(UserId, scope, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingReadDto>> GetById(Guid id, CancellationToken ct)
    {
        var res = await _svc.GetByIdAsync(UserId, id, ct);
        return res is null ? NotFound() : Ok(res);
    }

    [HttpPost]
    public async Task<ActionResult<BookingReadDto>> Create([FromBody] BookingCreateDto dto, CancellationToken ct)
    {
        var created = await _svc.CreateAsync(UserId, dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // “Cancel” istället för hard delete
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var ok = await _svc.CancelAsync(UserId, id, ct);
        return ok ? NoContent() : NotFound();
    }
}
