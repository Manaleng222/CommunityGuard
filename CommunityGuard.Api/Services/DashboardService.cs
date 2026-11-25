using CommunityGuard.Api.Data;
using CommunityGuard.Domain.Enums;
using CommunityGuard.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CommunityGuard.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context) => _context = context;

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        // 1. Get total open incidents.
        // AsNoTracking() is CRITICAL here. We are only reading. It makes the query faster.
        var totalOpen = await _context.Incidents
            .AsNoTracking()
            .CountAsync(i => i.Status != IncidentStatus.Closed);

        // 2. Get incidents grouped by sector.
        var bySector = await _context.Incidents
            .AsNoTracking()
            .GroupBy(i => i.Sector)
            .Select(g => new { Sector = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Sector, x => x.Count);

        // 3. Calculate Average Response Time.
        // We fetch the history into memory to do the math, as complex date math in LINQ-to-SQL can be slow.
        var history = await _context.IncidentStatusHistories
            .AsNoTracking()
            .Where(h => h.Status == IncidentStatus.Reported || h.Status == IncidentStatus.Acknowledged)
            .ToListAsync();

        // Group by IncidentId, find the time between Reported and Acknowledged.
        var responseTimes = history
            .GroupBy(h => h.IncidentId)
            .Select(g => {
                var reported = g.FirstOrDefault(h => h.Status == IncidentStatus.Reported)?.ChangedAt;
                var acknowledged = g.FirstOrDefault(h => h.Status == IncidentStatus.Acknowledged)?.ChangedAt;

                // If both exist, return the time difference in minutes.
                if (reported.HasValue && acknowledged.HasValue)
                    return (acknowledged.Value - reported.Value).TotalMinutes;

                return -1; // Indicates not acknowledged yet.
            })
            .Where(minutes => minutes >= 0)
            .ToList();

        var avgResponseTime = responseTimes.Any() ? responseTimes.Average() : 0;

        return new DashboardStatsDto(totalOpen, bySector, Math.Round(avgResponseTime, 1));
    }
}
