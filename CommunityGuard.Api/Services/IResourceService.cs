using CommunityGuard.Shared.Dtos;

namespace CommunityGuard.Api.Services;

public interface IResourceService
{
    Task<Guid> CreateAsync(CreateResourceRequest request);
    Task<IEnumerable<ResourceDto>> GetAllAsync();
    Task AllocateAsync(Guid resourceId, AllocateResourceRequest request);
}