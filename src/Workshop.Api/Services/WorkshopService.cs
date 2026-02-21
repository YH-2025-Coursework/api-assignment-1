using Microsoft.EntityFrameworkCore;
using Workshop.Api.Data;
using Workshop.Api.Dtos;
using WorkshopEntity = Workshop.Api.Entities.Workshop;

namespace Workshop.Api.Services;

// Concrete implementation that encapsulates EF Core CRUD + search logic.
// The primary constructor requests AppDbContext so DI injects the scoped context instance per request.
public sealed class WorkshopService(AppDbContext dbContext) : IWorkshopService
{
    /*
    `search` lets callers pass an optional query string (typically from GET /api/workshops?search=term).
    When a non-empty value comes in, the service applies a WHERE clause so only workshops whose titles contain that text are returned;
    if it’s null/whitespace, the method just returns the full list.
    */

    public async Task<IReadOnlyList<WorkshopResponseDto>> GetAllAsync(string? search, CancellationToken cancellationToken = default)
    {
        /*
        IQueryable<T> represents a deferred LINQ query against some data source (typically a database).
        Instead of executing immediately like IEnumerable<T>, it builds an expression tree that EF Core
        (or another provider) translates into SQL when it actually becomes enumerated (ToListAsync, FirstAsync, etc.).
        That lets us compose filters, ordering, projections, and have the provider run everything in the data store rather than in memory.

        AsNoTracking() tells EF Core not to track the entities it materializes from that query.
        Normally the DbContext keeps change-tracking information so it can detect updates and persist them later.
        When we’re only reading data (e.g., list/detail endpoints), tracking is wasted overhead.
        Calling AsNoTracking() returns a query that bypasses tracking, which reduces memory usage and speeds up read-only queries.
        */

        IQueryable<WorkshopEntity> query = dbContext.Workshops.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();
            query = query.Where(w => EF.Functions.Like(w.Title, $"%{normalizedSearch}%"));
        }

        var workshops = await query
            .OrderBy(w => w.Date)
            .ToListAsync(cancellationToken);

        return workshops.Select(MapToResponseDto).ToList();
    }

    // Define how the method fetches and shapes the data.
    public Task<WorkshopResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Workshops
            .AsNoTracking()
            .Where(w => w.Id == id)
            .Select(w => MapToResponseDto(w))  // For each w, convert it via MapToResponseDto(w).
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<WorkshopResponseDto> CreateAsync(WorkshopCreateDto createDto, CancellationToken cancellationToken = default)
    {
        var entity = new WorkshopEntity
        {
            Id = Guid.NewGuid(),
            Title = createDto.Title.Trim(),
            Description = createDto.Description.Trim(),
            Date = createDto.Date,
            MaxParticipants = createDto.MaxParticipants
        };

        dbContext.Workshops.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        /*
        Take the entity we just created/updated (WorkshopEntity) and run it through MapToResponseDto, which builds the
        response DTO (Id, Title, Date, MaxParticipants). The method then returns that DTO so callers get the shaped data
        instead of the raw entity.
        */

        return MapToResponseDto(entity);
    }

    public async Task<WorkshopResponseDto?> UpdateAsync(Guid id, WorkshopUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Workshops.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.Title = updateDto.Title.Trim();
        entity.Description = updateDto.Description.Trim();
        entity.Date = updateDto.Date;
        entity.MaxParticipants = updateDto.MaxParticipants;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponseDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Workshops.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        dbContext.Workshops.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /*
    Helper that copies the relevant fields from a WorkshopEntity into a new WorkshopResponseDto.
    It’s static because it doesn’t depend on instance state - just takes an entity and returns the DTO.
    All the service methods reuse it so response shaping stays consistent.
    */

    private static WorkshopResponseDto MapToResponseDto(WorkshopEntity entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Date = entity.Date,
        MaxParticipants = entity.MaxParticipants
    };
}

/*
Regarding `sealed` -  If that class wasn’t built with overridable members or extension points,
sealing it avoids subclasses trying to alter behavior that wasn’t designed to be safely altered.
*/