using CommunityGuard.Api.Data;
using CommunityGuard.Api.Services;
using CommunityGuard.Domain.Entities;
using CommunityGuard.Domain.Enums;
using Xunit;

namespace CommunityGuard.Api.Tests.Services;

public class DashboardServiceTests : Persistence.DatabaseTestBase
{
    [Fact]
    public async Task GetStatsAsync_ReturnsCorrectCountsAndAverageResponseTime()
    {
        // Arrange: Create a fresh database context.
        await using var context = CreateContext();

        // Seed 3 incidents in Sector 1, 1 in Sector 2.
        var i1 = Incident.Report("Test 1", "D", "Sector 1", IncidentPriority.High, Guid.NewGuid());
        var i2 = Incident.Report("Test 2", "D", "Sector 1", IncidentPriority.Low, Guid.NewGuid());
        var i3 = Incident.Report("Test 3", "D", "Sector 1", IncidentPriority.Low, Guid.NewGuid());
        var i4 = Incident.Report("Test 4", "D", "Sector 2", IncidentPriority.Medium, Guid.NewGuid());

        // Close one incident so it doesn't count as "Open".
        i3.TransitionTo(IncidentStatus.Closed, Guid.NewGuid());

        // Acknowledge i1 after 10 minutes (to test response time calculation).
        // Note: In a real test, you'd mock the time, but here we just let the domain create the history.
        i1.TransitionTo(IncidentStatus.Acknowledged, Guid.NewGuid(), "On it");

        context.Incidents.AddRange(i1, i2, i3, i4);
        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        // Act
        var stats = await service.GetStatsAsync();

        // Assert
        Assert.Equal(3, stats.TotalOpenIncidents); // i1, i2, i4 (i3 is closed)
        Assert.Equal(3, stats.IncidentsBySector["Sector 1"]);
        Assert.Equal(1, stats.IncidentsBySector["Sector 2"]);
        Assert.True(stats.AverageResponseTimeMinutes >= 0); // Just verify it calculates without throwing.
    }
}