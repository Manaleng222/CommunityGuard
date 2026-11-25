using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityGuard.Domain.Enums;

namespace CommunityGuard.Domain.Rules;

public static class IncidentStateMachine
{
    private static readonly Dictionary<IncidentStatus, IncidentStatus[]> AllowedTransitions = new()
    {
        [IncidentStatus.Reported] = new[] { IncidentStatus.Acknowledged, IncidentStatus.Closed },
        [IncidentStatus.Acknowledged] = new[] { IncidentStatus.Dispatched, IncidentStatus.Closed },
        [IncidentStatus.Dispatched] = new[] { IncidentStatus.Resolved },
        [IncidentStatus.Resolved] = new[] { IncidentStatus.Closed },
        [IncidentStatus.Closed] = Array.Empty<IncidentStatus>()
    };

    public static bool IsValidTransition(IncidentStatus from, IncidentStatus to)
        => AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);

    public static IEnumerable<IncidentStatus> GetAllowedNextStatuses(IncidentStatus from)
        => AllowedTransitions.TryGetValue(from, out var allowed)
            ? allowed
            : Array.Empty<IncidentStatus>();
}