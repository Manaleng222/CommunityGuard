

using CommunityGuard.Api.Data;
using CommunityGuard.Api.Services;
using CommunityGuard.Shared.Dtos;
using CommunityGuard.Domain.Enums;
using Xunit;

namespace CommunityGuard.Api.Tests.Services;

// Inherits DatabaseTestBase from Phase 2 to get a fresh in-memory DB.
public class IncidentServiceTests : Persistence.DatabaseTestBase
{
    [Fact]
    public async Task CreateAsync_WithValidRequest_SavesToDatabase()
    {
        // Arrange.
        await using var context = CreateContext();
        var service = new IncidentService(context);
        var request = new CreateIncidentRequest("Armed Robbery", "Two suspects", "Sector 1", IncidentPriority.Critical);
        var userId = Guid.NewGuid();

        // Act.
        var incidentId = await service.CreateAsync(request, userId);

        // Assert.
        var saved = await context.Incidents.FindAsync(incidentId);
        Assert.NotNull(saved);
        Assert.Equal("Armed Robbery", saved.Title);
        Assert.Equal(IncidentStatus.Reported, saved.Status);
    }
}
