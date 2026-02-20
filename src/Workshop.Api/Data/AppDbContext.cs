using Microsoft.EntityFrameworkCore;
// Alias entity types so I can reference them unambiguously inside DbContext
// despite the root namespace also being named Workshop.
using SessionEntity = Workshop.Api.Entities.Session;
using WorkshopEntity = Workshop.Api.Entities.Workshop;

namespace Workshop.Api.Data;

// EF Core DbContext describing the domain model and database connection.
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // Exposing EF Core DbSet properties for my entities so the context knows which tables exist.
    public DbSet<WorkshopEntity> Workshops => Set<WorkshopEntity>();
    public DbSet<SessionEntity> Sessions => Set<SessionEntity>();

    /*
    Each property calls the base DbContext.Set<TEntity>() method and returns the tracked set for that entity type.
    In practice:

    - EF Core discovers these properties to build the model.
    - Anywhere I receive AppDbContext, I can query/update those tables via LINQ.
    */
}