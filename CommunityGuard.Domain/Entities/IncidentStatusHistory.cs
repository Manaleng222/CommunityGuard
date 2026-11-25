using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityGuard.Domain.Enums;

namespace CommunityGuard.Domain.Entities;

public class IncidentStatusHistory
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid IncidentId { get; private set; }
    public IncidentStatus Status { get; private set; }
    public Guid ChangedByUserId { get; private set; }
    public string? Note { get; private set; }
    public DateTime ChangedAt { get; private set; } = DateTime.UtcNow;

    // For EF Core materialization in Phase 2.
    private IncidentStatusHistory() { }

    public IncidentStatusHistory(Guid incidentId, IncidentStatus status,
                                 Guid changedByUserId, string? note = null)
    {
        IncidentId = incidentId;
        Status = status;
        ChangedByUserId = changedByUserId;
        Note = note;
    }
}
