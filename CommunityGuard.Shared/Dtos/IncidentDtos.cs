using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityGuard.Domain.Enums;

namespace CommunityGuard.Shared.Dtos;

// What the API sends back to the client.
public record IncidentDto(
    Guid Id,
    string Title,
    string Description,
    string Sector,
    IncidentPriority Priority,
    IncidentStatus Status,
    DateTime CreatedAt);

// What the client sends when reporting a new incident.
public record CreateIncidentRequest(
    string Title,
    string Description,
    string Sector,
    IncidentPriority Priority);

// What the client sends when changing an incident's status.
public record UpdateIncidentStatusRequest(
    IncidentStatus NewStatus,
    string? Note);