using Microsoft.AspNetCore.Mvc;
using Workshop.Api.Dtos;
using Workshop.Api.Services;

namespace Workshop.Api.Controllers;

[ApiController]
[Route("api/workshops/{workshopId:guid}/sessions")]
// Hosts the nested /sessions endpoints for each workshop.
public class SessionsController(ISessionService sessionService) : ControllerBase
{
    [HttpGet]
    // GET /api/workshops/{workshopId}/sessions
    public async Task<ActionResult<IReadOnlyList<SessionResponseDto>>> GetAll(Guid workshopId, CancellationToken cancellationToken)
    {
        var sessions = await sessionService.GetAllAsync(workshopId, cancellationToken);
        if (sessions is null)
        {
            return NotFound();
        }

        return Ok(sessions);
    }

    [HttpGet("{sessionId:guid}")]
    // GET /api/workshops/{workshopId}/sessions/{sessionId}
    public async Task<ActionResult<SessionResponseDto>> GetById(Guid workshopId, Guid sessionId, CancellationToken cancellationToken)
    {
        var session = await sessionService.GetByIdAsync(workshopId, sessionId, cancellationToken);
        if (session is null)
        {
            return NotFound();
        }

        return Ok(session);
    }

    [HttpPost]
    // POST /api/workshops/{workshopId}/sessions
    public async Task<ActionResult<SessionResponseDto>> Create(Guid workshopId, [FromBody] SessionCreateDto createDto, CancellationToken cancellationToken)
    {
        var created = await sessionService.CreateAsync(workshopId, createDto, cancellationToken);
        if (created is null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(GetById), new { workshopId, sessionId = created.Id }, created);
    }

    [HttpPut("{sessionId:guid}")]
    // PUT /api/workshops/{workshopId}/sessions/{sessionId}
    public async Task<ActionResult<SessionResponseDto>> Update(Guid workshopId, Guid sessionId, [FromBody] SessionUpdateDto updateDto, CancellationToken cancellationToken)
    {
        var updated = await sessionService.UpdateAsync(workshopId, sessionId, updateDto, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{sessionId:guid}")]
    // DELETE /api/workshops/{workshopId}/sessions/{sessionId}
    public async Task<IActionResult> Delete(Guid workshopId, Guid sessionId, CancellationToken cancellationToken)
    {
        var deleted = await sessionService.DeleteAsync(workshopId, sessionId, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
