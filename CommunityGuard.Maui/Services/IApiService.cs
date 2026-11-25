using CommunityGuard.Shared.Common;
using CommunityGuard.Shared.Dtos;

namespace CommunityGuard.Maui.Services;

public interface IApiService
{
    Task<LoginResponse?> LoginAsync(string email, string password);
    Task<DashboardStatsDto?> GetDashboardStatsAsync();
    Task<PagedResult<IncidentDto>?> GetIncidentsAsync(int page, int pageSize);
    Task CreateIncidentAsync(CreateIncidentRequest request);

    // Use ONLY these names:
    Task<IncidentDetailDto?> GetIncidentByIdAsync(Guid id);
    Task UpdateIncidentStatusAsync(Guid id, UpdateIncidentStatusRequest request);

    Task<IEnumerable<ResourceDto>?> GetResourcesAsync();
    Task AllocateResourceAsync(Guid resourceId, AllocateResourceRequest request);
}