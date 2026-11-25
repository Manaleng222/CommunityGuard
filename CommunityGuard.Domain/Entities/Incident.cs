using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityGuard.Domain.Enums;
using CommunityGuard.Domain.Exceptions;
using CommunityGuard.Domain.Rules;

namespace CommunityGuard.Domain.Entities;

public class Incident : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Sector { get; private set; } = string.Empty;
    public IncidentPriority Priority { get; private set; } = IncidentPriority.Medium;
    public IncidentStatus Status { get; private set; } = IncidentStatus.Reported;
    public Guid ReportedByUserId { get; private set; }
    public Guid? AssignedToUserId { get; private set; }

    private readonly List<IncidentStatusHistory> _history = new();
    public IReadOnlyCollection<IncidentStatusHistory> History => _history.AsReadOnly();

    // For EF Core.
    private Incident() { }

    public static Incident Report(string title, string description, string sector,
                                  IncidentPriority priority, Guid reportedByUserId)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Title is required.");
        if (string.IsNullOrWhiteSpace(description)) throw new DomainException("Description is required.");
        if (string.IsNullOrWhiteSpace(sector)) throw new DomainException("Sector is required.");

        var incident = new Incident
        {
            Title = title.Trim(),
            Description = description.Trim(),
            Sector = sector.Trim(),
            Priority = priority,
            ReportedByUserId = reportedByUserId
        };

        incident._history.Add(
            new IncidentStatusHistory(incident.Id, IncidentStatus.Reported,
                                      reportedByUserId, "Incident reported."));

        return incident;
    }

    public void TransitionTo(IncidentStatus newStatus, Guid userId, string? note = null)
    {
        if (!IncidentStateMachine.IsValidTransition(Status, newStatus))
            throw new DomainException($"Cannot transition incident from {Status} to {newStatus}.");

        Status = newStatus;
        _history.Add(new IncidentStatusHistory(Id, newStatus, userId, note));
        MarkUpdated();
    }

    public void AssignTo(Guid userId)
    {
        if (Status == IncidentStatus.Closed)
            throw new DomainException("Cannot assign a closed incident.");

        AssignedToUserId = userId;
        MarkUpdated();
    }
}