using Microsoft.AspNetCore.Mvc;
using Workshop.Api.Dtos;
using Workshop.Api.Services;

namespace Workshop.Api.Controllers;

[ApiController]

/*
[Route("api/[controller]")] is a route template. ASP.NET Core replaces [controller] with the controller class name
minus the Controller suffix. For WorkshopsController, [controller] becomes workshops, so the base route becomes api/
workshops. Each action’s attributes (like [HttpGet], [HttpGet("{id:guid}")], etc.) append to that base to form the
final endpoint paths.
*/

[Route("api/[controller]")]
public class WorkshopsController(IWorkshopService workshopService) : ControllerBase  // ControllerBase comes from ASP.NET Core MVC.
{
    // GET /api/workshops?search=foo
    [HttpGet]

    /*
    ActionResult<T> is ASP.NET Core’s wrapper for “either T or an HTTP response”. Returning
    ActionResult<IReadOnlyList<WorkshopResponseDto>> lets the action return Ok(workshops) (which provides a 200 with that
    DTO list) or other results like NotFound()/BadRequest() while keeping the method signature strongly typed.

    [FromQuery] tells the model binder to pull the search value from the query string (?search=...) instead of the
    default sources. Without it, ASP.NET would still bind simple types from query by convention, but the attribute makes
    the intent explicit, especially when the action has multiple parameters coming from different sources (query, route,
    body).
    */

    public async Task<ActionResult<IReadOnlyList<WorkshopResponseDto>>> GetAll([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var workshops = await workshopService.GetAllAsync(search, cancellationToken);
        return Ok(workshops);
    }

    // GET /api/workshops/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkshopResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var workshop = await workshopService.GetByIdAsync(id, cancellationToken);
        if (workshop is null)
        {
            return NotFound();
        }

        return Ok(workshop);
    }

    // POST /api/workshops
    [HttpPost]
    public async Task<ActionResult<WorkshopResponseDto>> Create([FromBody] WorkshopCreateDto createDto, CancellationToken cancellationToken)
    {
        var created = await workshopService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/workshops/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<WorkshopResponseDto>> Update(Guid id, [FromBody] WorkshopUpdateDto updateDto, CancellationToken cancellationToken)
    {
        var updated = await workshopService.UpdateAsync(id, updateDto, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    // DELETE /api/workshops/{id}

    /*
    The DELETE action doesn’t return a DTO body - only status codes.
    GET/POST/PUT return actual WorkshopResponseDto payloads, so ActionResult<WorkshopResponseDto> conveys “either a DTO or another result”.
    Delete either returns NoContent() or NotFound(), both of which are plain HTTP results with no body, so IActionResult is
    sufficient (it’s the non-generic version when there’s no typed payload to describe).
    */

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await workshopService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
