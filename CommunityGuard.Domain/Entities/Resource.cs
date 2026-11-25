using CommunityGuard.Domain.Enums;
using CommunityGuard.Domain.Exceptions;
using CommunityGuard.Domain.Rules;

namespace CommunityGuard.Domain.Entities;

public class Resource : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public ResourceType Type { get; private set; }

    private readonly List<ResourceAllocation> _allocations = new();
    public IReadOnlyCollection<ResourceAllocation> Allocations => _allocations.AsReadOnly();

    private Resource() { }

    public static Resource Create(string name, ResourceType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Resource name is required.");

        return new Resource { Name = name.Trim(), Type = type };
    }

    public ResourceAllocation Allocate(Guid incidentId, DateTime fromUtc, DateTime toUtc)
    {
        ResourceAllocationRules.EnsureNoOverlap(_allocations, fromUtc, toUtc);

        var allocation = new ResourceAllocation(Id, incidentId, fromUtc, toUtc);
        _allocations.Add(allocation);
        MarkUpdated();
        return allocation;
    }
}