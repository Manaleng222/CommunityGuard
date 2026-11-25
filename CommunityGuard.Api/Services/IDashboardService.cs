using CommunityGuard.Shared.Dtos;

namespace CommunityGuard.Api.Services;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
}