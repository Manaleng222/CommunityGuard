using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityGuard.Domain.Enums;

namespace CommunityGuard.Shared.Dtos;

// What the API returns when asking for a specific incident.
public record IncidentDetailDto(
    Guid Id,
    string Title,
    string Description,
    string Sector,
    IncidentPriority Priority,
    IncidentStatus Status,
    DateTime CreatedAt,
    List<IncidentHistoryDto> History);

// A single entry in the incident's history/audit trail.
public record IncidentHistoryDto(
    IncidentStatus Status,
    DateTime ChangedAt,
    Guid ChangedByUserId,
    string? Note);
