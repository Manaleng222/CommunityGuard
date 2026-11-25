using CommunityGuard.Domain.Entities;
using CommunityGuard.Domain.Enums;
using CommunityGuard.Domain.Exceptions;
using Xunit;

namespace CommunityGuard.Domain.Tests;

public class IncidentTests
{
    private static Incident NewIncident() =>
        Incident.Report("Armed robbery", "Two suspects on foot", "Sector 1",
                        IncidentPriority.Critical, Guid.NewGuid());

    [Fact]
    public void Report_WithEmptyTitle_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Incident.Report("", "desc", "Sector 1", IncidentPriority.Low, Guid.NewGuid()));
    }

    [Fact]
    public void Report_WithEmptySector_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Incident.Report("Title", "desc", "  ", IncidentPriority.Low, Guid.NewGuid()));
    }

    [Fact]
    public void Report_CreatesIncidentWithReportedStatusAndInitialHistory()
    {
        var userId = Guid.NewGuid();
        var incident = Incident.Report("Armed robbery", "Two suspects on foot",
                                       "Sector 1", IncidentPriority.Critical, userId);

        Assert.Equal(IncidentStatus.Reported, incident.Status);
        Assert.Equal(IncidentPriority.Critical, incident.Priority);
        Assert.Single(incident.History);
        Assert.Equal(IncidentStatus.Reported, incident.History.First().Status);
        Assert.Equal(userId, incident.History.First().ChangedByUserId);
    }

    [Fact]
    public void TransitionTo_ValidPath_RecordsHistoryForEveryStep()
    {
        var userId = Guid.NewGuid();
        var incident = NewIncident();

        incident.TransitionTo(IncidentStatus.Acknowledged, userId, "Responder on the way");
        incident.TransitionTo(IncidentStatus.Dispatched, userId);
        incident.TransitionTo(IncidentStatus.Resolved, userId);
        incident.TransitionTo(IncidentStatus.Closed, userId);

        Assert.Equal(IncidentStatus.Closed, incident.Status);
        Assert.Equal(5, incident.History.Count); // initial + 4 transitions
    }

    [Fact]
    public void TransitionTo_IllegalTransition_ThrowsDomainException()
    {
        var userId = Guid.NewGuid();
        var incident = NewIncident();

        // Reported cannot jump straight to Dispatched.
        Assert.Throws<DomainException>(() =>
            incident.TransitionTo(IncidentStatus.Dispatched, userId));
    }

    [Fact]
    public void TransitionTo_FromClosed_AlwaysThrows()
    {
        var userId = Guid.NewGuid();
        var incident = NewIncident();
        incident.TransitionTo(IncidentStatus.Closed, userId);

        Assert.Throws<DomainException>(() =>
            incident.TransitionTo(IncidentStatus.Reported, userId));
    }

    [Fact]
    public void AssignTo_OpenIncident_SetsAssignedUserId()
    {
        var incident = NewIncident();
        var responder = Guid.NewGuid();

        incident.AssignTo(responder);

        Assert.Equal(responder, incident.AssignedToUserId);
    }

    [Fact]
    public void AssignTo_ClosedIncident_ThrowsDomainException()
    {
        var incident = NewIncident();
        incident.TransitionTo(IncidentStatus.Closed, Guid.NewGuid());

        Assert.Throws<DomainException>(() => incident.AssignTo(Guid.NewGuid()));
    }
}