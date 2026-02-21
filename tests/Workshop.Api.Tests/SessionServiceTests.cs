using Microsoft.EntityFrameworkCore;
using Workshop.Api.Data;
using Workshop.Api.Dtos;
using Workshop.Api.Services;
using SessionEntity = Workshop.Api.Entities.Session;
using WorkshopEntity = Workshop.Api.Entities.Workshop;

namespace Workshop.Api.Tests;

// Validates the child-entity CRUD logic handled by SessionService.
public class SessionServiceTests
{
    // Should return null (so controller can emit 404) when the parent workshop does not exist.
    [Fact]
    public async Task GetAllAsync_ReturnsNull_WhenWorkshopMissing()
    {
        await using var context = CreateContext();
        var service = new SessionService(context);

        var sessions = await service.GetAllAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(sessions);
    }

    // Sessions must be scoped and ordered by their parent workshop.
    [Fact]
    public async Task GetAllAsync_ReturnsSessionsForWorkshop()
    {
        await using var context = CreateContext();
        var workshopId = await SeedWorkshopAsync(context);
        await SeedSessionsAsync(context, workshopId);
        var service = new SessionService(context);

        var sessions = await service.GetAllAsync(workshopId, CancellationToken.None);

        Assert.NotNull(sessions);
        Assert.Equal(2, sessions.Count);
        Assert.True(sessions[0].StartTime <= sessions[1].StartTime);
    }

    // CreateAsync must persist trimmed titles and attach the new session to the workshop.
    [Fact]
    public async Task CreateAsync_PersistsSessionForWorkshop()
    {
        await using var context = CreateContext();
        var workshopId = await SeedWorkshopAsync(context);
        var service = new SessionService(context);
        var dto = new SessionCreateDto
        {
            Title = " Kickoff ",
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(3)
        };

        var created = await service.CreateAsync(workshopId, dto, CancellationToken.None);

        Assert.NotNull(created);
        Assert.Equal("Kickoff", created!.Title);
        Assert.Equal(workshopId, created.WorkshopId);

        var saved = await context.Sessions.SingleAsync(s => s.Id == created.Id);
        Assert.Equal("Kickoff", saved.Title);
    }

    // Spin up an isolated in-memory DbContext for each test so they do not share state.
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    // Create a single workshop and returns its ID, giving tests a valid parent record.
    private static async Task<Guid> SeedWorkshopAsync(AppDbContext context)
    {
        var workshop = new WorkshopEntity
        {
            Id = Guid.NewGuid(),
            Title = "VG Workshop",
            Description = "Testing sessions",
            Date = DateTime.UtcNow.AddDays(1),
            MaxParticipants = 10
        };

        context.Workshops.Add(workshop);
        await context.SaveChangesAsync();
        return workshop.Id;
    }

    // Use the workshop ID to insert two sessions with known titles/times.
    private static async Task SeedSessionsAsync(AppDbContext context, Guid workshopId)
    {
        var sessions = new[]
        {
            new SessionEntity
            {
                Id = Guid.NewGuid(),
                WorkshopId = workshopId,
                Title = "Intro",
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2)
            },
            new SessionEntity
            {
                Id = Guid.NewGuid(),
                WorkshopId = workshopId,
                Title = "Deep dive",
                StartTime = DateTime.UtcNow.AddHours(3),
                EndTime = DateTime.UtcNow.AddHours(4)
            }
        };

        context.Sessions.AddRange(sessions);
        await context.SaveChangesAsync();
    }
}
