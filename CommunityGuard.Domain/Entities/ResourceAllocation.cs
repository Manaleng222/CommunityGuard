using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityGuard.Domain.Exceptions;

namespace CommunityGuard.Domain.Entities;

public class ResourceAllocation
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ResourceId { get; private set; }
    public Guid IncidentId { get; private set; }
    public DateTime FromUtc { get; private set; }
    public DateTime ToUtc { get; private set; }

    private ResourceAllocation() { }

    public ResourceAllocation(Guid resourceId, Guid incidentId, DateTime fromUtc, DateTime toUtc)
    {
        if (toUtc <= fromUtc)
            throw new DomainException("Allocation end must be after start.");

        ResourceId = resourceId;
        IncidentId = incidentId;
        FromUtc = fromUtc;
        ToUtc = toUtc;
    }

    public bool OverlapsWith(DateTime fromUtc, DateTime toUtc)
        => FromUtc < toUtc && fromUtc < ToUtc;
}
