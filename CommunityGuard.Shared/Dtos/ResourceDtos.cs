using CommunityGuard.Domain.Enums;

namespace CommunityGuard.Shared.Dtos;

// What the API returns when listing resources.
public record ResourceDto(Guid Id, string Name, ResourceType Type);

// What the client sends to create a new resource.
public record CreateResourceRequest(string Name, ResourceType Type);

// What the client sends to dispatch a resource to an incident.
public record AllocateResourceRequest(Guid IncidentId, DateTime FromUtc, DateTime ToUtc);
