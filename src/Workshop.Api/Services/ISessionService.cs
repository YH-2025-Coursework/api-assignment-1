using Workshop.Api.Dtos;

namespace Workshop.Api.Services;

// Abstraction for CRUD operations scoped to a workshop's sessions.
public interface ISessionService
{
    Task<IReadOnlyList<SessionResponseDto>?> GetAllAsync(Guid workshopId, CancellationToken cancellationToken = default);
    Task<SessionResponseDto?> GetByIdAsync(Guid workshopId, Guid sessionId, CancellationToken cancellationToken = default);
    Task<SessionResponseDto?> CreateAsync(Guid workshopId, SessionCreateDto createDto, CancellationToken cancellationToken = default);
    Task<SessionResponseDto?> UpdateAsync(Guid workshopId, Guid sessionId, SessionUpdateDto updateDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid workshopId, Guid sessionId, CancellationToken cancellationToken = default);
}
