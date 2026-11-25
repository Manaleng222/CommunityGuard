using CommunityGuard.Shared.Common;
using CommunityGuard.Shared.Dtos;

namespace CommunityGuard.Api.Services;

public interface IIncidentService
{
    Task<Guid> CreateAsync(CreateIncidentRequest request, Guid userId);

    Task<PagedResult<IncidentDto>> GetPagedAsync(int page, int pageSize, Guid userId, bool isController);
    //Task<PagedResult<IncidentDto>> GetPagedAsync(int page, int pageSize);
    Task UpdateStatusAsync(Guid incidentId, UpdateIncidentStatusRequest request, Guid userId);
    Task<IncidentDetailDto?> GetByIdAsync(Guid id);
}
