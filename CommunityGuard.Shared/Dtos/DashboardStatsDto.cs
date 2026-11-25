namespace CommunityGuard.Shared.Dtos;

// The shape of the data sent to the MAUI Dashboard.
public record DashboardStatsDto(
    int TotalOpenIncidents,
    Dictionary<string, int> IncidentsBySector,
    double AverageResponseTimeMinutes);
