using Application.Category.DTO;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // lås CRUD till Admins
public sealed class CategoriesController : ControllerBase
{
    private readonly ICategoryService _svc;

    public CategoriesController(ICategoryService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetAll(CancellationToken ct)
        => Ok(await _svc.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryReadDto>> GetById(Guid id, CancellationToken ct)
    {
        var dto = await _svc.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CategoryCreateDto dto, CancellationToken ct)
    {
        try
        {
            var id = await _svc.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryUpdateDto dto, CancellationToken ct)
    {
        try
        {
            return await _svc.UpdateAsync(id, dto, ct) ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _svc.DeleteAsync(id, ct) ? NoContent() : NotFound();

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        => await _svc.SoftDeactivateAsync(id, ct) ? NoContent() : NotFound();
}
