using CommunityGuard.Api.Data;
using CommunityGuard.Domain.Entities;
using CommunityGuard.Domain.Enums;
using CommunityGuard.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CommunityGuard.Api.Tests.Persistence;

public class IncidentPersistenceTests : DatabaseTestBase
{
    [Fact]
    public async Task SaveIncident_PersistsWithHistory()
    {
        // Arrange.
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        var incident = Incident.Report("Armed robbery", "Two suspects on foot",
                                       "Sector 1", IncidentPriority.Critical, userId);

        // Act.
        context.Incidents.Add(incident);
        await context.SaveChangesAsync();

        // Assert: reload from a fresh context (simulates a new HTTP request).
        await using var verify = CreateContext();
        var saved = await verify.Incidents
            .Include(i => i.History)
            .SingleAsync(i => i.Id == incident.Id);

        Assert.Equal(IncidentStatus.Reported, saved.Status);
        Assert.Single(saved.History);   // Initial history row was saved.
    }

    [Fact]
    public async Task SoftDelete_ExcludesIncidentFromQueries()
    {
        // Arrange: save an incident, then soft-delete it.
        await using var context = CreateContext();
        var incident = Incident.Report("Test", "Test desc", "Sector 2",
                                       IncidentPriority.Low, Guid.NewGuid());
        context.Incidents.Add(incident);
        await context.SaveChangesAsync();

        incident.SoftDelete();          // Domain method flips IsDeleted.
        await context.SaveChangesAsync();

        // Act: query normally — global filter should hide it.
        await using var verify = CreateContext();
        var found = await verify.Incidents.AnyAsync(i => i.Id == incident.Id);

        // Assert: the query filter returned nothing.
        Assert.False(found);
    }

    [Fact]
    public async Task ToPagedResultAsync_ReturnsCorrectPageAndTotal()
    {
        // Arrange: seed 25 incidents.
        await using var context = CreateContext();
        for (var i = 0; i < 25; i++)
        {
            context.Incidents.Add(Incident.Report($"Incident {i}", "Desc",
                "Sector 3", IncidentPriority.Medium, Guid.NewGuid()));
        }
        await context.SaveChangesAsync();

        // Act: request page 2 with 10 per page.
        var page = await context.Incidents
            .OrderBy(i => i.Title)
            .ToPagedResultAsync(page: 2, pageSize: 10);

        // Assert.
        Assert.Equal(25, page.TotalCount);
        Assert.Equal(10, page.Items.Count);
        Assert.Equal(3, page.TotalPages);   // 25 items / 10 per page = 3 pages.
    }
}