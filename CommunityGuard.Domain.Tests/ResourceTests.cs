
using CommunityGuard.Domain.Entities;
using CommunityGuard.Domain.Enums;
using CommunityGuard.Domain.Exceptions;
using Xunit;

namespace CommunityGuard.Domain.Tests;

public class ResourceTests
{
    [Fact]
    public void Create_WithEmptyName_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Resource.Create("", ResourceType.PatrolVehicle));
    }

    [Fact]
    public void Create_WithValidName_TrimsNameAndSetsType()
    {
        var resource = Resource.Create("  Patrol 1  ", ResourceType.PatrolVehicle);

        Assert.Equal("Patrol 1", resource.Name);
        Assert.Equal(ResourceType.PatrolVehicle, resource.Type);
    }

    [Fact]
    public void Allocate_WhenResourceIsFree_AddsAllocation()
    {
        var resource = Resource.Create("Patrol 1", ResourceType.PatrolVehicle);
        var now = DateTime.UtcNow;

        var allocation = resource.Allocate(Guid.NewGuid(), now, now.AddHours(2));

        Assert.Single(resource.Allocations);
        Assert.Equal(resource.Id, allocation.ResourceId);
    }

    [Fact]
    public void Allocate_WhenAlreadyBooked_ThrowsDomainException()
    {
        var resource = Resource.Create("Patrol 1", ResourceType.PatrolVehicle);
        var now = DateTime.UtcNow;
        resource.Allocate(Guid.NewGuid(), now, now.AddHours(2));

        Assert.Throws<DomainException>(() =>
            resource.Allocate(Guid.NewGuid(), now.AddHours(1), now.AddHours(3)));
    }

    [Fact]
    public void Allocate_WithAdjacentWindow_Succeeds()
    {
        var resource = Resource.Create("Patrol 1", ResourceType.PatrolVehicle);
        var now = DateTime.UtcNow;
        resource.Allocate(Guid.NewGuid(), now, now.AddHours(2));

        // Starts exactly when the previous ends — should not overlap.
        resource.Allocate(Guid.NewGuid(), now.AddHours(2), now.AddHours(3));

        Assert.Equal(2, resource.Allocations.Count);
    }
}