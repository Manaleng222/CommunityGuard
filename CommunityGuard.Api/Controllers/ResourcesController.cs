using CommunityGuard.Api.Services;
using CommunityGuard.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityGuard.Api.Controllers;

[Authorize]
public class ResourcesController : BaseController
{
    private readonly IResourceService _resourceService;

    public ResourcesController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateResourceRequest request)
    {
        var resourceId = await _resourceService.CreateAsync(request);
        return Ok(new { id = resourceId });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var resources = await _resourceService.GetAllAsync();
        return Ok(resources);
    }

    [HttpPost("{id}/allocate")]
    public async Task<IActionResult> Allocate(Guid id, AllocateResourceRequest request)
    {
        // The service will throw a DomainException if the resource is double-booked.
        // Our Global Middleware from Phase 3 will catch it and return a 400 Bad Request.
        await _resourceService.AllocateAsync(id, request);
        return Ok(new { message = "Resource successfully allocated." });
    }
}