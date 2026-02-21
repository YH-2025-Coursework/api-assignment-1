using Microsoft.EntityFrameworkCore;
using Workshop.Api.Data;
using Workshop.Api.Dtos;
using SessionEntity = Workshop.Api.Entities.Session;

namespace Workshop.Api.Services;

// Encapsulates CRUD operations for workshop sessions via EF Core.
public sealed class SessionService(AppDbContext dbContext) : ISessionService
{
    /*
    IReadOnlyList<T> is the interface that exposes read-only list semantics (indexing + Count) without committing to
    a concrete type. C# doesn’t have a ReadOnlyList<T> class baked in. Returning the interface lets us materialize
    the results into any List<T>/ReadOnlyCollection<T> internally while signaling to callers they shouldn’t modify the
    collection. It’s a common pattern for service APIs when we just need to expose a list of DTOs without allowing mutation.
    */
    public async Task<IReadOnlyList<SessionResponseDto>?> GetAllAsync(Guid workshopId, CancellationToken cancellationToken = default)
    {
        if (!await WorkshopExists(workshopId, cancellationToken))
        {
            return null;
        }

        var sessions = await dbContext.Sessions
            .AsNoTracking()
            .Where(s => s.WorkshopId == workshopId)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);

        return sessions.Select(MapToResponseDto).ToList();
    }

    public async Task<SessionResponseDto?> GetByIdAsync(Guid workshopId, Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await dbContext.Sessions
            .AsNoTracking()
            .Where(s => s.WorkshopId == workshopId && s.Id == sessionId)
            .SingleOrDefaultAsync(cancellationToken);

        return session is null ? null : MapToResponseDto(session);
    }

    public async Task<SessionResponseDto?> CreateAsync(Guid workshopId, SessionCreateDto createDto, CancellationToken cancellationToken = default)
    {
        if (!await WorkshopExists(workshopId, cancellationToken))
        {
            return null;
        }

        var entity = new SessionEntity
        {
            Id = Guid.NewGuid(),
            WorkshopId = workshopId,
            Title = createDto.Title.Trim(),
            StartTime = createDto.StartTime,
            EndTime = createDto.EndTime
        };

        dbContext.Sessions.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponseDto(entity);
    }

    public async Task<SessionResponseDto?> UpdateAsync(Guid workshopId, Guid sessionId, SessionUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Sessions
            .FirstOrDefaultAsync(s => s.WorkshopId == workshopId && s.Id == sessionId, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        entity.Title = updateDto.Title.Trim();
        entity.StartTime = updateDto.StartTime;
        entity.EndTime = updateDto.EndTime;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponseDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid workshopId, Guid sessionId, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Sessions
            .FirstOrDefaultAsync(s => s.WorkshopId == workshopId && s.Id == sessionId, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        dbContext.Sessions.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private Task<bool> WorkshopExists(Guid workshopId, CancellationToken cancellationToken) =>
        dbContext.Workshops.AnyAsync(w => w.Id == workshopId, cancellationToken);

    private static SessionResponseDto MapToResponseDto(SessionEntity entity) => new()
    {
        Id = entity.Id,
        WorkshopId = entity.WorkshopId,
        Title = entity.Title,
        StartTime = entity.StartTime,
        EndTime = entity.EndTime
    };
}
