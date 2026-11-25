using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityGuard.Domain.Entities;
using CommunityGuard.Domain.Exceptions;

namespace CommunityGuard.Domain.Rules;

public static class ResourceAllocationRules
{
    public static void EnsureNoOverlap(IEnumerable<ResourceAllocation> existing,
                                       DateTime fromUtc, DateTime toUtc)
    {
        if (existing.Any(a => a.OverlapsWith(fromUtc, toUtc)))
            throw new DomainException("Resource is already allocated during the requested time range.");
    }
}