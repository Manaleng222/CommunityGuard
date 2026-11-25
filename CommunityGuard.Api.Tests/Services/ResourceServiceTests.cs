using CommunityGuard.Api.Data;
using CommunityGuard.Api.Services;
using CommunityGuard.Domain.Entities;
using CommunityGuard.Domain.Enums;
using CommunityGuard.Domain.Exceptions;
using CommunityGuard.Shared.Dtos;
using System.ComponentModel.Design;
using Xunit;

namespace CommunityGuard.Api.Tests.Services;

public class ResourceServiceTests : Persistence.DatabaseTestBase
{
    [Fact]
    public async Task AllocateAsync_OverlappingTimeRange_ThrowsDomainException()
    {
        // Arrange: Create a resource and a booking in the database.
        await using var context = CreateContext();
        var resource = Resource.Create("Patrol 1", ResourceType.PatrolVehicle);
        var now = DateTime.UtcNow;

        // Book it from now to +2 hours.
        resource.Allocate(Guid.NewGuid(), now, now.AddHours(2));
        context.Resources.Add(resource);
        await context.SaveChangesAsync();

        var service = new ResourceService(context);

        // Act & Assert: Try to book it overlapping (now +1 hour to +3 hours).
        var overlappingRequest = new AllocateResourceRequest(Guid.NewGuid(), now.AddHours(1), now.AddHours(3));

        await Assert.ThrowsAsync<DomainException>(() =>
            service.AllocateAsync(resource.Id, overlappingRequest));
    }
}