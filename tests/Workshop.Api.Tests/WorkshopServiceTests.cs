using Microsoft.EntityFrameworkCore;
using Workshop.Api.Data;
using Workshop.Api.Dtos;
using Workshop.Api.Services;
using WorkshopEntity = Workshop.Api.Entities.Workshop;

namespace Workshop.Api.Tests;

// Covers WorkshopService behavior end-to-end via EF Core's in-memory provider so no SQL Server is needed.
public class WorkshopServiceTests
{
    // Ensures the search filter limits results to titles that contain the query text.
    [Fact]
    public async Task GetAllAsync_FiltersByTitle_WhenSearchProvided()
    {
        await using var context = CreateContext();
        await SeedWorkshopsAsync(context);
        var service = new WorkshopService(context);

        var results = await service.GetAllAsync("Intro", CancellationToken.None);

        Assert.Single(results);
        Assert.Equal("Intro to EF Core", results[0].Title);
    }

    // When no search term is supplied, the service should return all workshops ordered by date.
    [Fact]
    public async Task GetAllAsync_ReturnsAll_WhenSearchMissing()
    {
        await using var context = CreateContext();
        await SeedWorkshopsAsync(context);
        var service = new WorkshopService(context);

        var results = await service.GetAllAsync(null, CancellationToken.None);

        Assert.Equal(2, results.Count);
        Assert.True(results[0].Date <= results[1].Date, "Results should be ordered by Date ascending.");
    }

    // Verifies CreateAsync trims whitespace and actually persists the entity.
    [Fact]
    public async Task CreateAsync_PersistsWorkshopWithTrimmedFields()
    {
        await using var context = CreateContext();
        var service = new WorkshopService(context);
        var createDto = new WorkshopCreateDto
        {
            Title = "  Trim me ",
            Description = "  Body ",
            Date = DateTime.UtcNow.AddDays(5),
            MaxParticipants = 10
        };

        var created = await service.CreateAsync(createDto, CancellationToken.None);

        Assert.Equal("Trim me", created.Title);

        var saved = await context.Workshops.SingleAsync(w => w.Id == created.Id);
        Assert.Equal("Trim me", saved.Title);
        Assert.Equal("Body", saved.Description);
    }

    // Spin up an isolated in-memory DbContext for each test so they do not share state.
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    // Seeds deterministic data so tests can assert ordering/filtering against predictable values.
    private static async Task SeedWorkshopsAsync(AppDbContext context)
    {
        var workshops = new[]
        {
            new WorkshopEntity
            {
                Id = Guid.NewGuid(),
                Title = "Intro to EF Core",
                Description = "Basics of EF Core",
                Date = DateTime.UtcNow.AddDays(1),
                MaxParticipants = 10
            },
            new WorkshopEntity
            {
                Id = Guid.NewGuid(),
                Title = "Advanced ASP.NET",
                Description = "Deep dive",
                Date = DateTime.UtcNow.AddDays(2),
                MaxParticipants = 20
            }
        };

        await context.Workshops.AddRangeAsync(workshops);
        await context.SaveChangesAsync();
    }
}
