using CommunityGuard.Api.Data;
using CommunityGuard.Domain.Entities;
using CommunityGuard.Domain.Exceptions;
using CommunityGuard.Shared.Dtos;


using Microsoft.EntityFrameworkCore;

namespace CommunityGuard.Api.Services;

public class ResourceService : IResourceService
{
    private readonly AppDbContext _context;

    public ResourceService(AppDbContext context) => _context = context;

    public async Task<Guid> CreateAsync(CreateResourceRequest request)
    {
        // 1. Use the Domain factory to create the resource.
        var resource = Resource.Create(request.Name, request.Type);

        // 2. Save to database.
        _context.Resources.Add(resource);
        await _context.SaveChangesAsync();

        return resource.Id;
    }

    public async Task<IEnumerable<ResourceDto>> GetAllAsync()
    {
        // Query and project directly to DTOs.
        return await _context.Resources
            .Select(r => new ResourceDto(r.Id, r.Name, r.Type))
            .ToListAsync();
    }

    public async Task AllocateAsync(Guid resourceId, AllocateResourceRequest request)
    {
        // 1. Fetch the resource AND its existing allocations.
        // CRITICAL: We must use .Include() so the Domain can check for overlaps!
        var resource = await _context.Resources
            .Include(r => r.Allocations)
            .FirstOrDefaultAsync(r => r.Id == resourceId);

        if (resource == null)
            throw new DomainException("Resource not found.");

        // 2. Call the Domain method. It will throw a DomainException if there is an overlap.
        resource.Allocate(request.IncidentId, request.FromUtc, request.ToUtc);

        // 3. Save the new allocation to the database.
        await _context.SaveChangesAsync();
    }
}