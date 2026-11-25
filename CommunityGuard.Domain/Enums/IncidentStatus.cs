using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// File: Enums/IncidentStatus.cs
namespace CommunityGuard.Domain.Enums;

public enum IncidentStatus
{
    Reported = 1,
    Acknowledged = 2,
    Dispatched = 3,
    Resolved = 4,
    Closed = 5
}