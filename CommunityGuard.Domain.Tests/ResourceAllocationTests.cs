using CommunityGuard.Domain.Entities;
using CommunityGuard.Domain.Exceptions;
using CommunityGuard.Domain.Rules;
using System.Reflection;
using Xunit;

namespace CommunityGuard.Domain.Tests;

public class ResourceAllocationTests
{
    [Fact]
    public void Constructor_WithEndBeforeStart_ThrowsDomainException()
    {
        var now = DateTime.UtcNow;
        Assert.Throws<DomainException>(() =>
            new ResourceAllocation(Guid.NewGuid(), Guid.NewGuid(), now, now.AddMinutes(-1)));
    }

    [Fact]
    public void EnsureNoOverlap_WithOverlappingWindow_ThrowsDomainException()
    {
        var now = DateTime.UtcNow;
        var existing = new[]
        {
            new ResourceAllocation(Guid.NewGuid(), Guid.NewGuid(), now, now.AddHours(2))
        };

        Assert.Throws<DomainException>(() =>
            ResourceAllocationRules.EnsureNoOverlap(existing, now.AddHours(1), now.AddHours(3)));
    }

    [Fact]
    public void EnsureNoOverlap_WithNonOverlappingWindow_Succeeds()
    {
        var now = DateTime.UtcNow;
        var existing = new[]
        {
            new ResourceAllocation(Guid.NewGuid(), Guid.NewGuid(), now, now.AddHours(2))
        };

        // Should not throw.
        ResourceAllocationRules.EnsureNoOverlap(existing, now.AddHours(3), now.AddHours(4));
    }

    [Fact]
    public void EnsureNoOverlap_WithTouchingWindow_Succeeds()
    {
        var now = DateTime.UtcNow;
        var existing = new[]
        {
            new ResourceAllocation(Guid.NewGuid(), Guid.NewGuid(), now, now.AddHours(2))
        };

        // A window starting exactly when the other ends does not overlap.
        ResourceAllocationRules.EnsureNoOverlap(existing, now.AddHours(2), now.AddHours(3));
    }
}