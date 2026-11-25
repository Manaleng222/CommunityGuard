using CommunityGuard.Domain.Enums;
using CommunityGuard.Domain.Rules;
using Xunit;

namespace CommunityGuard.Domain.Tests;

public class IncidentStateMachineTests
{
    [Theory]
    [InlineData(IncidentStatus.Reported, IncidentStatus.Acknowledged, true)]
    [InlineData(IncidentStatus.Reported, IncidentStatus.Closed, true)]
    [InlineData(IncidentStatus.Reported, IncidentStatus.Dispatched, false)]
    [InlineData(IncidentStatus.Acknowledged, IncidentStatus.Dispatched, true)]
    [InlineData(IncidentStatus.Acknowledged, IncidentStatus.Reported, false)]
    [InlineData(IncidentStatus.Dispatched, IncidentStatus.Resolved, true)]
    [InlineData(IncidentStatus.Resolved, IncidentStatus.Closed, true)]
    [InlineData(IncidentStatus.Closed, IncidentStatus.Reported, false)]
    [InlineData(IncidentStatus.Closed, IncidentStatus.Closed, false)]
    public void IsValidTransition_ReturnsExpected(IncidentStatus from, IncidentStatus to, bool expected)
    {
        Assert.Equal(expected, IncidentStateMachine.IsValidTransition(from, to));
    }

    [Fact]
    public void GetAllowedNextStatuses_FromReported_ReturnsAcknowledgedAndClosed()
    {
        var allowed = IncidentStateMachine.GetAllowedNextStatuses(IncidentStatus.Reported).ToList();

        Assert.Contains(IncidentStatus.Acknowledged, allowed);
        Assert.Contains(IncidentStatus.Closed, allowed);
        Assert.DoesNotContain(IncidentStatus.Dispatched, allowed);
    }
}